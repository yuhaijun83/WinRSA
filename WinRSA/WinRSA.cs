using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using WinRSA.Properties;

namespace WinRSA
{
    public partial class WinRSA : Form
    {
        public WinRSA()
        {
            InitializeComponent();
        }

        private void TabSetMode()
        {
            this.TabControlMain.DrawMode = TabDrawMode.OwnerDrawFixed;
            //this.TabControlMain.Alignment = TabAlignment.Left;
            //this.TabControlMain.SizeMode = TabSizeMode.Fixed;
            //this.TabControlMain.Multiline = true;
            //this.TabControlMain.ItemSize = new Size(50, 100);
        }

        private static void Init()
        {
            try
            {
                //Create a UnicodeEncoder to convert between byte array and string.
                UnicodeEncoding ByteConverter = new UnicodeEncoding();

                //Create byte arrays to hold original, encrypted, and decrypted data.
                byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
                byte[] encryptedData;
                byte[] decryptedData;

                int iRSA_Key_Length = 2048;

                //Create a new instance of RSACryptoServiceProvider to generate
                //public and private key data.
                using (RSACryptoServiceProvider RSA = GetRSAInstance())
                {
                    RSAParameters rsaKeyInfo = RSA.ExportParameters(false);                   


                    //Pass the data to ENCRYPT, the public key information 
                    //(using RSACryptoServiceProvider.ExportParameters(false),
                    //and a boolean flag specifying no OAEP padding.
                    encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);

                    //Pass the data to DECRYPT, the private key information 
                    //(using RSACryptoServiceProvider.ExportParameters(true),
                    //and a boolean flag specifying no OAEP padding.
                    decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                    //Display the decrypted plaintext to the console. 
                    Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
                }
            }
            catch (ArgumentNullException)
            {
                //Catch this exception in case the encryption did
                //not succeed.
                Console.WriteLine("Encryption failed.");
            }
        }

        private static RSACryptoServiceProvider GetRSAInstance()
        {
            int iRSA_Key_Length = 2048;

            //Create a new instance of RSACryptoServiceProvider to generate
            //public and private key data.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(iRSA_Key_Length))
            {
                // the public key information
                RSAParameters publicRSAKeyInfo = RSA.ExportParameters(false);

                // the private key information
                RSAParameters privateRSAKeyInfo = RSA.ExportParameters(true);


                return RSA;
            }

        }

        private static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo)
        {
            return RSAEncrypt(DataToEncrypt, RSAKeyInfo, true);
        }

        private static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = GetRSAInstance())
                {

                    //Import the RSA Key information. This only needs
                    //to include the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        private static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo)
        {
            return RSADecrypt(DataToDecrypt, RSAKeyInfo, true);
        }

        private static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = GetRSAInstance())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        private void WinRSA_Load(object sender, EventArgs e)
        {
            this.Text = "WinRSA " + Resources.WinRSA_Build_Version;
            TabSetMode();
        }

        private void TabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            Rectangle tabArea = this.TabControlMain.GetTabRect(e.Index);
            Graphics g = e.Graphics;

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;

            Font font = this.TabControlMain.Font;
            SolidBrush brush = new SolidBrush(Color.Black);
            g.DrawString(((TabControl)(sender)).TabPages[e.Index].Text, font, brush, tabArea, sf);
        }
    }

}
