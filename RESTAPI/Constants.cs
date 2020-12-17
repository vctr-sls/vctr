using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTAPI
{
    public static class Constants
    {
        public const string ConfigKeyPasswordHashingDegreeOfParallelism = "PasswordHashing:DegreeOfParallelism";
        public const string ConfigKeyPasswordHashingMemoryPoolKB = "PasswordHashing:MemoryPoolKB";
        public const string ConfigKeyPasswordHashingIterations = "PasswordHashing:Iterations";
        public const string ConfigKeyPasswordHashingSaltLength = "PasswordHashing:SaltLength";
        public const string ConfigKeyPasswordHashingKeyLength = "PasswordHashing:KeyLength";

        public const string ConfigKeyInitializationUserName = "Initialization:RootUserName";
        public const string ConfigKeyInitializationPassword = "Initialization:RootUserPassword";
    }
}
