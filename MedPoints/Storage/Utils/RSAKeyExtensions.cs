using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace Storage.Utils
{
    public class RSAParametersJson
    {
        public string Modulus { get; set; }
        public string Exponent { get; set; }
        public string P { get; set; }
        public string Q { get; set; }
        public string DP { get; set; }
        public string DQ { get; set; }
        public string InverseQ { get; set; }
        public string D { get; set; }
    }


    public static class RsaKeyExtensions
    {      
        public static void FromJsonString(this RSA rsa, string jsonString)
        {
            try
            {
                var paramsJson = JsonConvert.DeserializeObject<RSAParametersJson>(jsonString);

                var parameters = new RSAParameters
                {
                    Modulus = paramsJson.Modulus != null ? Convert.FromBase64String(paramsJson.Modulus) : null,
                    Exponent = paramsJson.Exponent != null ? Convert.FromBase64String(paramsJson.Exponent) : null,
                    P = paramsJson.P != null ? Convert.FromBase64String(paramsJson.P) : null,
                    Q = paramsJson.Q != null ? Convert.FromBase64String(paramsJson.Q) : null,
                    DP = paramsJson.DP != null ? Convert.FromBase64String(paramsJson.DP) : null,
                    DQ = paramsJson.DQ != null ? Convert.FromBase64String(paramsJson.DQ) : null,
                    InverseQ = paramsJson.InverseQ != null ? Convert.FromBase64String(paramsJson.InverseQ) : null,
                    D = paramsJson.D != null ? Convert.FromBase64String(paramsJson.D) : null
                };

                rsa.ImportParameters(parameters);
            }
            catch
            {
                throw new Exception("Invalid JSON RSA key.");
            }
        }

        public static string ToJsonString(this RSA rsa, bool includePrivateParameters)
        {
            var parameters = rsa.ExportParameters(includePrivateParameters);

            var parasJson = new RSAParametersJson()
            {
                Modulus = parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                Exponent = parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                P = parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                Q = parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                DP = parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                DQ = parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                InverseQ = parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                D = parameters.D != null ? Convert.ToBase64String(parameters.D) : null
            };

            return JsonConvert.SerializeObject(parasJson);
        }
    }
}
