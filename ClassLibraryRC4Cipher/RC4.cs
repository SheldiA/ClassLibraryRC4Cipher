using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibraryLFSRCipher;
using System.Windows.Forms;

namespace ClassLibraryRC4Cipher
{
    class RC4 : MarshalByRefObject, ICiphers
    {
        public string AlgorithmName { get; set; }
        private byte[] password;
        private byte[] sBox;
        int countI, countJ;

        private delegate void ProggressBarWork(ProgressBar pb);

        public RC4(byte[] password)
        {
            AlgorithmName = "RC4";
            this.password = password;
            sBox = new byte[256];
            //this.panelControls = panelControls;
        }

        private void InitializeSbox()
        {
            int j = 0;
            for (int i = 0; i < 256; ++i)
                sBox[i] = (byte)i;
            for (int i = 0; i < 256; ++i)
            {
                j = (j + sBox[i] + password[i % password.Length]) % 256;
                Swap(sBox, i, j);
            }
        }

        private byte GetNextKeyByte()
        {
            byte keyByte;
            int index;

            countI = (countI + 1) % 256;
            countJ = (countJ + sBox[countI]) % 256;
            Swap(sBox, countI, countJ);
            index = (sBox[countI] + sBox[countJ]) % 256;
            keyByte = sBox[index];

            return keyByte;
        }

        public byte[] Encrypt(byte[] incomingBytes, object pb)
        {
            ProgressBar pbar = pb as ProgressBar;
            byte[] resultBytes = new byte[incomingBytes.Length];
            countI = 0;
            countJ = 0;
            InitializeSbox();
            for (int i = 0; i < incomingBytes.Length; ++i)
            {
                pbar.Invoke(new ProggressBarWork((p) => p.PerformStep()), pbar);
                resultBytes[i] = (byte)(incomingBytes[i] ^ GetNextKeyByte());
            }

            return resultBytes;
        }

        public byte[] Decrypt(byte[] incomingBytes, object pb)
        {
            return Encrypt(incomingBytes, pb);
        }

        private void Swap(byte[] array, int i, int j)
        {
            byte temp;
            temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
