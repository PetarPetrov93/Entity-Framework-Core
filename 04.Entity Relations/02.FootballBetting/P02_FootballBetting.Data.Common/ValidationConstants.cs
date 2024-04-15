using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Common
{
    public class ValidationConstants
    {
        //Team
        public const int TeamNameLength = 70;
        public const int LogoUrlLength = 2048;
        public const int InitialsLength = 4;

        //Color
        public const int ColorLength = 20;

        //Town
        public const int TownNameLength = 100;

        //Country
        public const int CountryNameLength = 30;

        //Player
        public const int PlayerNameLength = 70;

        //Position
        public const int PositionNameLength = 30;

        //User
        public const int UsernameLength = 50;
        public const int NameOfUserLength = 50;
        public const int PasswordLength = 256;
        public const int EmailLength = 256;

        //Game
        public const int ResultLength = 7;
    }
}
