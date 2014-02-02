using System.Collections.Generic;
using FluentNHibernate.Mapping;
using ServiceStack.ServiceInterface.Auth;

namespace ServiceStack.Authentication.NHibernate
{
    public class UserOAuthProviderMap : ClassMap<UserOAuthProviderPersistenceDto>
    {
        public UserOAuthProviderMap()
        {
            Table("UserOAuthProvider");
            Id(x => x.Id)
                .GeneratedBy.Native();

            Map(x => x.AccessToken);
            Map(x => x.AccessTokenSecret);
            Map(x => x.CreatedDate);
            Map(x => x.DisplayName);
            Map(x => x.Email);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.ModifiedDate);
            Map(x => x.Provider);
            Map(x => x.RequestToken);
            Map(x => x.RequestTokenSecret);
            Map(x => x.UserAuthId);
            Map(x => x.UserId);
            Map(x => x.UserName);

            HasMany(x => x.Items1)
                .AsMap<string>(
                    index => index.Column("`Key`").Type<string>(),
                    element => element.Column("Value").Type<string>())
                .KeyColumn("UserOAuthProviderID")
                .Table("UserOAuthProvider_Items")
                .Not.LazyLoad()
                .Cascade.All();

        }

    }

    public class UserOAuthProviderPersistenceDto : UserOAuthProvider
    {
        public UserOAuthProviderPersistenceDto()
            : base()
        { }

        public UserOAuthProviderPersistenceDto(UserOAuthProvider userOAuthProvider)
        {
            Id = userOAuthProvider.Id;
            UserAuthId = userOAuthProvider.UserAuthId;
            Provider = userOAuthProvider.Provider;
            UserId = userOAuthProvider.UserId;
            UserName = userOAuthProvider.UserName;
            DisplayName = userOAuthProvider.DisplayName;
            FirstName = userOAuthProvider.FirstName;
            LastName = userOAuthProvider.LastName;
            Email = userOAuthProvider.Email;
            RequestToken = userOAuthProvider.RequestToken;
            RequestTokenSecret = userOAuthProvider.RequestTokenSecret;
            Items = userOAuthProvider.Items;
            AccessToken = userOAuthProvider.AccessToken;
            AccessTokenSecret = userOAuthProvider.AccessTokenSecret;
            CreatedDate = userOAuthProvider.CreatedDate;
            ModifiedDate = userOAuthProvider.ModifiedDate;
        }

        public virtual IDictionary<string, string> Items1
        {
            get { return Items; }
            set { Items = new Dictionary<string, string>(value) ; }
        }
    
    }

}