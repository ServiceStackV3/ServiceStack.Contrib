using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ServiceStack.Common;
using ServiceStack.Text;
using ServiceStack.ServiceInterface.Auth;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace ServiceStack.Authentication.MongoDB
{
	public class MongoDBAuthRepository : IUserAuthRepository, IClearable
	{

		//http://stackoverflow.com/questions/3588623/c-sharp-regex-for-a-username-with-a-few-restrictions
		public Regex ValidUserNameRegEx = new Regex(@"^(?=.{3,15}$)([A-Za-z0-9][._-]?)*$", RegexOptions.Compiled);

		private readonly MongoDatabase mongoDatabase;

		public MongoDBAuthRepository(MongoDatabase mongoDatabase)
		{
			this.mongoDatabase = mongoDatabase;
		}

		public void CreateMissingTables()
		{
			if(!mongoDatabase.CollectionExists("UserAuth"))
				mongoDatabase.CreateCollection("UserAuth");

			if (!mongoDatabase.CollectionExists("UserOAuthProvider"))
				mongoDatabase.CreateCollection("UserOAuthProvider");
		}

		public void DropAndReCreateTables()
		{
			if (mongoDatabase.CollectionExists("UserAuth"))
				mongoDatabase.DropCollection("UserAuth");
			mongoDatabase.CreateCollection("UserAuth");

			if (mongoDatabase.CollectionExists("UserOAuthProvider"))
				mongoDatabase.DropCollection("UserOAuthProvider");
			mongoDatabase.CreateCollection("UserOAuthProvider");
		}

		private void ValidateNewUser(UserAuth newUser, string password)
		{
			newUser.ThrowIfNull("newUser");
			password.ThrowIfNullOrEmpty("password");

			if (newUser.UserName.IsNullOrEmpty() && newUser.Email.IsNullOrEmpty())
				throw new ArgumentNullException("UserName or Email is required");

			if (!newUser.UserName.IsNullOrEmpty())
			{
				if (!ValidUserNameRegEx.IsMatch(newUser.UserName))
					throw new ArgumentException("UserName contains invalid characters", "UserName");
			}
		}

		public UserAuth CreateUserAuth(UserAuth newUser, string password)
		{
			ValidateNewUser(newUser, password);

			AssertNoExistingUser(mongoDatabase, newUser);

			var saltedHash = new SaltedHash();
			string salt;
			string hash;
			saltedHash.GetHashAndSaltString(password, out hash, out salt);
            var digestHelper = new DigestAuthFunctions();
            newUser.DigestHA1Hash = digestHelper.CreateHa1(newUser.UserName, DigestAuthProvider.Realm, password);
			newUser.PasswordHash = hash;
			newUser.Salt = salt;
			newUser.CreatedDate = DateTime.UtcNow;
			newUser.ModifiedDate = newUser.CreatedDate;

			var collection = mongoDatabase.GetCollection<UserAuth>("UserAuth");
			collection.Insert(newUser);
			// todo - update id here
			return newUser;
		}

		private static void AssertNoExistingUser(MongoDatabase mongoDatabase, UserAuth newUser, UserAuth exceptForExistingUser = null)
		{
			if (newUser.UserName != null)
			{
				var existingUser = GetUserAuthByUserName(mongoDatabase, newUser.UserName);
				if (existingUser != null
					&& (exceptForExistingUser == null || existingUser.Id != exceptForExistingUser.Id))
					throw new ArgumentException("User {0} already exists".Fmt(newUser.UserName));
			}
			if (newUser.Email != null)
			{
				var existingUser = GetUserAuthByUserName(mongoDatabase, newUser.Email);
				if (existingUser != null
					&& (exceptForExistingUser == null || existingUser.Id != exceptForExistingUser.Id))
					throw new ArgumentException("Email {0} already exists".Fmt(newUser.Email));
			}
		}

		public UserAuth UpdateUserAuth(UserAuth existingUser, UserAuth newUser, string password)
		{
			ValidateNewUser(newUser, password);

			AssertNoExistingUser(mongoDatabase, newUser, existingUser);

			var hash = existingUser.PasswordHash;
			var salt = existingUser.Salt;
            if (password != null)
			{
				var saltedHash = new SaltedHash();
				saltedHash.GetHashAndSaltString(password, out hash, out salt);
			}
            // If either one changes the digest hash has to be recalculated
            var digestHash = existingUser.DigestHA1Hash;
            if (password != null || existingUser.UserName != newUser.UserName)
            {
                var digestHelper = new DigestAuthFunctions();
                digestHash = digestHelper.CreateHa1(newUser.UserName, DigestAuthProvider.Realm, password);
            }
			newUser.Id = existingUser.Id;
			newUser.PasswordHash = hash;
			newUser.Salt = salt;
            newUser.DigestHA1Hash = digestHash;
			newUser.CreatedDate = existingUser.CreatedDate;
			newUser.ModifiedDate = DateTime.UtcNow;

			var collection = mongoDatabase.GetCollection<UserAuth>("UserAuth");
			collection.Insert(newUser);

			return newUser;
		}

		public UserAuth GetUserAuthByUserName(string userNameOrEmail)
		{
			return GetUserAuthByUserName(mongoDatabase, userNameOrEmail);
		}

		private static UserAuth GetUserAuthByUserName(MongoDatabase mongoDatabase, string userNameOrEmail)
		{
			var isEmail = userNameOrEmail.Contains("@");
			var collection = mongoDatabase.GetCollection<UserAuth>("UserAuth");

			QueryComplete query = isEmail
				? Query.EQ("Email", userNameOrEmail)
				: Query.EQ("UserName", userNameOrEmail) ;

			UserAuth userAuth = collection.FindOne(query);
			return userAuth;
		}

		public bool TryAuthenticate(string userName, string password, out UserAuth userAuth)
		{
			//userId = null;
			userAuth = GetUserAuthByUserName(userName);
			if (userAuth == null) return false;

			var saltedHash = new SaltedHash();
			if (saltedHash.VerifyHashString(password, userAuth.PasswordHash, userAuth.Salt))
			{
				//userId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
				return true;
			}

			userAuth = null;
			return false;
		}
        public bool TryAuthenticate(Dictionary<string,string> digestHeaders, string PrivateKey, int NonceTimeOut, string sequence, out UserAuth userAuth)
        {
            //userId = null;
            userAuth = GetUserAuthByUserName(digestHeaders["username"]);
            if (userAuth == null) return false;

            var digestHelper = new DigestAuthFunctions();
            if (digestHelper.ValidateResponse(digestHeaders,PrivateKey,NonceTimeOut,userAuth.DigestHA1Hash,sequence))
            {
                //userId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
                return true;
            }
            userAuth = null;
            return false;
        }

		public void LoadUserAuth(IAuthSession session, IOAuthTokens tokens)
		{
			session.ThrowIfNull("session");

			var userAuth = GetUserAuth(session, tokens);
			LoadUserAuth(session, userAuth);
		}

		private void LoadUserAuth(IAuthSession session, UserAuth userAuth)
		{
			if (userAuth == null) return;

			session.PopulateWith(userAuth);
            session.UserAuthId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
            session.ProviderOAuthAccess = GetUserOAuthProviders(session.UserAuthId)
				.ConvertAll(x => (IOAuthTokens)x);
			
		}

		public UserAuth GetUserAuth(string userAuthId)
		{
			var collection = mongoDatabase.GetCollection<UserAuth>("UserAuth");
			UserAuth userAuth = collection.FindOneById(userAuthId);
			return userAuth;
		}

		public void SaveUserAuth(IAuthSession authSession)
		{

			var userAuth = !authSession.UserAuthId.IsNullOrEmpty()
				? GetUserAuth(authSession.UserAuthId)
				: authSession.TranslateTo<UserAuth>();

			if (userAuth.Id == default(int) && !authSession.UserAuthId.IsNullOrEmpty())
				userAuth.Id = int.Parse(authSession.UserAuthId);

			userAuth.ModifiedDate = DateTime.UtcNow;
			if (userAuth.CreatedDate == default(DateTime))
				userAuth.CreatedDate = userAuth.ModifiedDate;

			var collection = mongoDatabase.GetCollection<UserAuth>("UserAuth");
			collection.Insert(userAuth);
		}

		public void SaveUserAuth(UserAuth userAuth)
		{
			userAuth.ModifiedDate = DateTime.UtcNow;
			if (userAuth.CreatedDate == default(DateTime))
				userAuth.CreatedDate = userAuth.ModifiedDate;

			var collection = mongoDatabase.GetCollection<UserAuth>("UserAuth");
			collection.Insert(userAuth);
		}

		public List<UserOAuthProvider> GetUserOAuthProviders(string userAuthId)
		{
			var id = int.Parse(userAuthId);

			QueryComplete query = Query.EQ("UserAuthId", userAuthId);

			var collection = mongoDatabase.GetCollection<UserOAuthProvider>("UserOAuthProvider");
			MongoCursor<UserOAuthProvider> queryResult = collection.Find(query);
			return queryResult.ToList();

		}

		public UserAuth GetUserAuth(IAuthSession authSession, IOAuthTokens tokens)
		{
			if (!authSession.UserAuthId.IsNullOrEmpty())
			{
				var userAuth = GetUserAuth(authSession.UserAuthId);
				if (userAuth != null) return userAuth;
			}
			if (!authSession.UserAuthName.IsNullOrEmpty())
			{
				var userAuth = GetUserAuthByUserName(authSession.UserAuthName);
				if (userAuth != null) return userAuth;
			}

			if (tokens == null || tokens.Provider.IsNullOrEmpty() || tokens.UserId.IsNullOrEmpty())
				return null;

			var query = Query.And(
							Query.EQ("Provider", tokens.Provider),
							Query.EQ("UserId", tokens.UserId)
						);

			var providerCollection = mongoDatabase.GetCollection<UserOAuthProvider>("UserOAuthProvider");
			var oAuthProvider = providerCollection.FindOne(query);


			if (oAuthProvider != null)
			{
				var userAuthCollection = mongoDatabase.GetCollection<UserAuth>("UserAuth");
				var userAuth = userAuthCollection.FindOneById(oAuthProvider.UserAuthId);
				return userAuth;
			}
			return null;
		}

		public string CreateOrMergeAuthSession(IAuthSession authSession, IOAuthTokens tokens)
		{
			var userAuth = GetUserAuth(authSession, tokens) ?? new UserAuth();

			var query = Query.And(
							Query.EQ("Provider", tokens.Provider),
							Query.EQ("UserId", tokens.UserId)
						); 
			var providerCollection = mongoDatabase.GetCollection<UserOAuthProvider>("UserOAuthProvider");
			var oAuthProvider = providerCollection.FindOne(query);

			if (oAuthProvider == null)
			{
				oAuthProvider = new UserOAuthProvider {
					Provider = tokens.Provider,
					UserId = tokens.UserId,
				};
			}

			oAuthProvider.PopulateMissing(tokens);
			userAuth.PopulateMissing(oAuthProvider);

			userAuth.ModifiedDate = DateTime.UtcNow;
			if (userAuth.CreatedDate == default(DateTime))
				userAuth.CreatedDate = userAuth.ModifiedDate;

			var userAuthCollection = mongoDatabase.GetCollection<UserAuth>("UserAuth");

			userAuthCollection.Save(userAuth);

			oAuthProvider.UserAuthId = userAuth.Id;

			if (oAuthProvider.CreatedDate == default(DateTime))
				oAuthProvider.CreatedDate = userAuth.ModifiedDate;
			oAuthProvider.ModifiedDate = userAuth.ModifiedDate;

			providerCollection.Save(oAuthProvider);

			return oAuthProvider.UserAuthId.ToString(CultureInfo.InvariantCulture);
		}

		public void Clear()
		{
			DropAndReCreateTables();
		}
	}
}
