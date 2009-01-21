using System;

namespace Parasite
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ParasiteGame game = new ParasiteGame())
            {
                game.Run();
            }
        }
    }
}

