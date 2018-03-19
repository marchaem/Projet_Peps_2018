using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Clients
    {
        public List<Client> ObtenirListeClients()
        {
            return new List<Client>
        {
            new Client { Age = 33, Nom = "Nicolas"},
            new Client { Age = 30, Nom = "Delphine"},
            new Client { Age = 33, Nom = "Jérémie"},
            new Client { Age = 1, Nom = "Timéo"}
        };
        }

    }
}