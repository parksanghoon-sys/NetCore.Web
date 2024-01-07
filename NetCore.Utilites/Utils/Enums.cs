namespace NetCore.Utilites.Utils
{
    public class Enums
    {
        /// <summary>
        /// 암호화 유형
        /// </summary>
        public enum CryptoType
        {
            Unmanaged = 1,
            Managed,
            CngCbc,
            CngGcm
        }
    }
}
