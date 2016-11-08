using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public class SystemActors
    {
        public static IActorRef CommandProcessor { get; set; }
    }

    public class NameRegistry
    {
        public static string AccountIndexProjection = "AccountIndexProjection";
    }
}
