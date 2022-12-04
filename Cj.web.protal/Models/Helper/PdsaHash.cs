using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Cj.web.Models
{
    public class PdsaHash
        {
            private PDSAHashType mbytHashType;
            private string mstrOriginalString;
            private string mstrHashString;
            private HashAlgorithm mhash;
            bool mboolUseSalt;
            string mstrSaltValue = String.Empty;
            short msrtSaltLength = 8;

            public enum PDSAHashType : byte
            {
                MD5,
                SHA1,
                SHA256,
                SHA384,
                SHA512,
                SHA513
            }

            #region "Public Properties"
            public PDSAHashType HashType
            {
                get { return mbytHashType; }
                set
                {
                    if (mbytHashType != value)
                    {
                        mbytHashType = value;
                        mstrOriginalString = String.Empty;
                        mstrHashString = String.Empty;

                        this.SetEncryptor();
                    }
                }
            }

            public string SaltValue
            {
                get { return mstrSaltValue; }
                set { mstrSaltValue = value; }
            }

            public bool UseSalt
            {
                get { return mboolUseSalt; }
                set { mboolUseSalt = value; }
            }

            public short SaltLength
            {
                get { return msrtSaltLength; }
                set { msrtSaltLength = value; }
            }

            public string OriginalString
            {
                get { return mstrOriginalString; }
                set { mstrOriginalString = value; }
            }

            public string HashString
            {
                get { return mstrHashString; }
                set { mstrHashString = value; }
            }
            #endregion

            #region "Constructors"
            public PdsaHash()
            {
                mbytHashType = PDSAHashType.MD5;
            }

            public PdsaHash(PDSAHashType HashType)
            {
                mbytHashType = HashType;
            }

            public PdsaHash(PDSAHashType HashType,
              string OriginalString)
            {
                mbytHashType = HashType;
                mstrOriginalString = OriginalString;
            }

            public PdsaHash(PDSAHashType HashType,
              string OriginalString,
              bool UseSalt,
              string SaltValue)
            {
                mbytHashType = HashType;
                mstrOriginalString = OriginalString;
                mboolUseSalt = UseSalt;
                mstrSaltValue = SaltValue;
            }

            #endregion

            #region "SetEncryptor() Method"

            private void SetEncryptor()
            {
                switch (mbytHashType)
                {
                    case PDSAHashType.MD5:
                        mhash = new MD5CryptoServiceProvider();
                        break;
                    case PDSAHashType.SHA1:
                        mhash = new SHA1CryptoServiceProvider();
                        break;
                    case PDSAHashType.SHA256:
                        mhash = new SHA256Managed();
                        break;
                    case PDSAHashType.SHA384:
                        mhash = new SHA384Managed();
                        break;
                    case PDSAHashType.SHA512:
                        mhash = new SHA512Managed();
                        break;
                }
            }
            #endregion

            #region "CreateHash() Methods"
            public string CreateHash()
            {
                byte[] bytValue;
                byte[] bytHash;

                // Create New Crypto Service Provider Object
                this.SetEncryptor();

                // Check to see if we will Salt the value
                if (mboolUseSalt)
                    if (mstrSaltValue.Length == 0)
                        mstrSaltValue = this.CreateSalt();

                // Convert the original string to array of Bytes
                bytValue =
                  System.Text.Encoding.UTF8.GetBytes(
                  mstrSaltValue + mstrOriginalString);

                // Compute the Hash, returns an array of Bytes  
                bytHash = mhash.ComputeHash(bytValue);

                // Return a base 64 encoded string of the Hash value
                return Convert.ToBase64String(bytHash);
            }

            public string CreateHash(string OriginalString)
            {
                mstrOriginalString = OriginalString;
                return this.CreateHash();
            }

            public string CreateHash(string OriginalString,
              PDSAHashType HashType)
            {
                mstrOriginalString = OriginalString;
                mbytHashType = HashType;
                return this.CreateHash();
            }

            public string CreateHash(string OriginalString,
              bool UseSalt)
            {
                mstrOriginalString = OriginalString;
                mboolUseSalt = UseSalt;
                return this.CreateHash();
            }

            public string CreateHash(string OriginalString,
              PDSAHashType HashType,
              string SaltValue)
            {
                mstrOriginalString = OriginalString;
                mbytHashType = HashType;
                mstrSaltValue = SaltValue;
                return this.CreateHash();
            }

            public string CreateHash(string OriginalString,
              string SaltValue)
            {
                mstrOriginalString = OriginalString;
                mstrSaltValue = SaltValue;
                return this.CreateHash();
            }
            #endregion


            public void Reset()
            {
                mstrSaltValue = String.Empty;
                mstrOriginalString = String.Empty;
                mstrHashString = String.Empty;
                mboolUseSalt = false;
                mbytHashType = PDSAHashType.MD5;
                mhash = null;
            }

            public string CreateSalt()
            {
                byte[] bytSalt = new byte[8];
                RNGCryptoServiceProvider rng;
                rng = new RNGCryptoServiceProvider();
                rng.GetBytes(bytSalt);
                return Convert.ToBase64String(bytSalt);
            }
        }
    }
