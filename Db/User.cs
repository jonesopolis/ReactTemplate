using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Db {
    public class User : IdentityUser
    {
        public ICollection<Contact> Contacts { get; set; }
    }
}