using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class FurnitureGenerator
    {
        Random random;

        List<String> gravestoneInscriptions;
        List<String> signInscriptions;

        public FurnitureGenerator()
        {
            random = new Random();
            gravestoneInscriptions = new List<string>();
            signInscriptions = new List<string>();
            LoadInscriptions();
        }

        public Furniture randomFurniture()
        {
            Furniture result = new Furniture();
            int furnType = random.Next(SC.numFurnTypes);
            //result.furnType = (int)SC.FurnTypes.ALTAR;
            result.furnType = furnType;

            switch (furnType)
            {
                case (int)SC.FurnTypes.ALTAR:
                    result.burnable = false;
                    result.inscribed = false;
                    result.name = "Altar";
                    break;
                case (int)SC.FurnTypes.BARREL:
                    result.burnable = true;
                    result.inscribed = false;
                    result.name = "Barrel";
                    break;
                case (int)SC.FurnTypes.CANDELABRA:
                    result.burnable = false;
                    result.inscribed = false;
                    result.name = "Candelabra";
                    break;
                case (int)SC.FurnTypes.CRATE:
                    result.burnable = true;
                    result.inscribed = false;
                    result.CanSeeThrough = false;
                    result.name = "Crate";
                    break;
                case (int)SC.FurnTypes.GRAVESTONE:
                    result.burnable = false;
                    result.inscribed = true;
                    result.CanSeeThrough = false;
                    result.name = "Gravestone";
                    result.inscription = GraveInscription();
                    break;
                case (int)SC.FurnTypes.SIGNPOST:
                    result.burnable = true;
                    result.inscribed = true;
                    result.name = "Signpost";
                    result.inscription = SignInscription();
                    break;
                case (int)SC.FurnTypes.STATUE:
                    result.burnable = false;
                    result.inscribed = false;
                    result.CanSeeThrough = false;
                    result.name = "Statue";
                    break;
                case (int)SC.FurnTypes.TABLE:
                    result.burnable = true;
                    result.inscribed = false;
                    result.name = "Table";
                    break;
                case (int)SC.FurnTypes.CHAIR_LEFT:
                    result.burnable = true;
                    result.inscribed = false;
                    result.name = "Chair";
                    break;
                case (int)SC.FurnTypes.CHAIR_RIGHT:
                    result.burnable = true;
                    result.inscribed = false;
                    result.name = "Chair";
                    break;
                case (int)SC.FurnTypes.TELEPORTER: //Don't want to have random teleporters. Turn into a gravestone instead
                    result.furnType = (int)SC.FurnTypes.GRAVESTONE;
                    result.burnable = false;
                    result.inscribed = true;
                    result.CanSeeThrough = false;
                    result.name = "Gravestone";
                    result.inscription = GraveInscription();
                    break;
                default:
                    result.inscribed = false;
                    result.name = "DEFAULT FURNITURE";
                    break;
            }

            return result;
        }

        public Furniture GenerateSignpost(int x, int y, String inscription)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.SIGNPOST;
            result.burnable = true;
            result.inscribed = true;
            result.name = "Signpost";
            result.x = x;
            result.y = y;
            result.inscription = inscription;
            return result;
        }
        public Furniture GenerateTable(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.TABLE;
            result.burnable = true;
            result.inscribed = false;
            result.name = "Table";
            result.x = x;
            result.y = y;
            return result;
        }
        public Furniture GenerateLeftFacingChair(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.CHAIR_LEFT;
            result.burnable = true;
            result.inscribed = false;
            result.name = "Chair";
            result.x = x;
            result.y = y;
            return result;
        }
        public Furniture GenerateRightFacingChair(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.CHAIR_RIGHT;
            result.burnable = true;
            result.inscribed = false;
            result.name = "Chair";
            result.x = x;
            result.y = y;
            return result;
        }
        public Furniture GenerateTeleporter(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.TELEPORTER;
            result.burnable = false;
            result.inscribed = false;
            result.CanWalkOver = true;
            result.name = "Teleporter";
            result.x = x;
            result.y = y;
            return result;
        }
        public Furniture GenerateStatue(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.STATUE;
            result.burnable = false;
            result.inscribed = false;
            result.CanSeeThrough = false;
            result.name = "Statue";
            result.x = x;
            result.y = y;
            return result;
        }
        public Furniture GenerateCrate(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.CRATE;
            result.burnable = true;
            result.inscribed = false;
            result.CanSeeThrough = false;
            result.name = "Crate";
            result.x = x;
            result.y = y;
            return result;
        }
        public Furniture GenerateBarrel(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.BARREL;
            result.burnable = true;
            result.inscribed = false;
            result.name = "Barrel";
            result.x = x;
            result.y = y;
            return result;
        }
        public Furniture GenerateCandelabra(int x, int y)
        {
            Furniture result = new Furniture();
            result.furnType = (int)SC.FurnTypes.CANDELABRA;
            result.burnable = false;
            result.inscribed = false;
            result.name = "Candelabra";
            result.x = x;
            result.y = y;
            return result;
        }

        private String GraveInscription()
        {
            return gravestoneInscriptions[random.Next(gravestoneInscriptions.Count)];
        }
        private String SignInscription()
        {
            return signInscriptions[random.Next(signInscriptions.Count)];
        }

        private void LoadInscriptions()
        {
            gravestoneInscriptions.Add("R.I.P.");
            gravestoneInscriptions.Add("R.I.P.");
            gravestoneInscriptions.Add("R.I.P.");
            gravestoneInscriptions.Add("R.I.P.");
            gravestoneInscriptions.Add("R.I.P.");
            gravestoneInscriptions.Add("A Brave Villager Lies Here - RIP");
            gravestoneInscriptions.Add("Here Lies Tony, Killed By A Goblin");
            gravestoneInscriptions.Add("Here Lies An Unknown Elf, Found Killed By An Arrow");
            gravestoneInscriptions.Add("The Person In This Coffin Is Alive.");
            gravestoneInscriptions.Add("Here Lies Steven. Or At Least, His Body. We Found It Over There");
            gravestoneInscriptions.Add("Here Lies Error");
            gravestoneInscriptions.Add("Here Lies Jane, Died Of Cholera");
            gravestoneInscriptions.Add("Here lies andy... peperony and chease");
            gravestoneInscriptions.Add("Here lies K Project, killed by kicking a wall");
            gravestoneInscriptions.Add("Here Lies A Father Of 29. He Would Have Had More But He Ran Out Of Time");
            gravestoneInscriptions.Add("Here Lies The Body Of Our Anna; Done To Death By A Banana");
            gravestoneInscriptions.Add("Here Lies Jonathan Fiddle; Went Out Of Tune");
            gravestoneInscriptions.Add("Here Lies Landlord Tommy Dent, In His Last Cosy Tenement");
            gravestoneInscriptions.Add("Here Lies Candle Jack; Beloved Son, Brother And Husb");
            gravestoneInscriptions.Add("Thanks For Reading This Grave. Now Leave - You're Standing On My Head!");;
            gravestoneInscriptions.Add("Here Lies Dan - Surprised And Killed By A Manticore");

            signInscriptions.Add("Beware Of The Leopard");
            signInscriptions.Add("Adventurers Eat Corpses At Their Own Risk");
            signInscriptions.Add("NO LOITERING");
            signInscriptions.Add("GOOBLNS RUEL OK");
            signInscriptions.Add("WARNING: Traps laid in this dungeon");
            signInscriptions.Add("Property of Gold Rick");
            signInscriptions.Add("Anyone threatening the Empire of Jem will be executed");
            signInscriptions.Add("Aiding rebels is treason");
            signInscriptions.Add("WANTED: DEAD - Village rebels. 500 gold a head.");
            signInscriptions.Add("Please respect the sanctity of altars.");
        }

    }
}
