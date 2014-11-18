using System.Collections.Generic;
using Amazon.IdentityManagement.Model;
using Nancy.Security;
using TreeGecko.Library.Common.Enums;

namespace Site.Objects
{
    public class NancyUser : IUserIdentity
    {
        private readonly List<string> m_Claims;
 
        public NancyUser()
        {
            m_Claims = new List<string>();
        }

        public IEnumerable<string> Claims
        {
            get { return m_Claims; }
        }

        public string UserName { get; set; }

        public bool IsAdmin
        {
            get
            {
                return m_Claims.Contains("Admin");
            }
        }

        public bool IsSuperUser
        {
            get
            {
                return m_Claims.Contains("SuperUser");
            }
        }

        public void SetClaims(UserTypes _userTypes)
        {
            switch (_userTypes)
                {
                    case UserTypes.User:
                        m_Claims.Add("User");
                        break;
                    case UserTypes.SuperUser:
                        m_Claims.Add("User");
                        m_Claims.Add("SuperUser");
                        break;
                    case UserTypes.Administrator:
                        m_Claims.Add("User");
                        m_Claims.Add("SuperUser");
                        m_Claims.Add("Administrator");
                        break;
                }
            
        }
    }
}