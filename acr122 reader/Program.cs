


using System;
using System.Linq;
using System.Text;

using PCSC;

public class ACR122UReader
{
    private readonly PCSC.ISCardContext _context;

    public ACR122UReader()
    {
        _context = ContextFactory.Instance.Establish(SCardScope.User);
    }

    public string ReadCardUID(string cardType, byte[] command)
    {
        PCSC.ICardReader reader = null;

        try
        {
            // Get the list of available readers
            string[] readerNames = _context.GetReaders();

            // Check if any readers are connected
            if (!readerNames.Any())
            {
                throw new Exception("No readers found");
            }
            var readerFirstname = readerNames[0];
            // Connect to the first reader (modify if needed)
            reader = _context.ConnectReader(readerFirstname, SCardShareMode.Exclusive,SCardProtocol.Any);
            // Connect to the reader
            //reader.Connect(SCardConnect.Exclusive);

            // Send APDU command based on card type
            byte[] response=new byte[6];
            string data="";
               switch (cardType.ToLower())
               {
                   case "mifare classic":
                       reader.Transmit(command,response);
                        data = ProcessResponse(response, cardType);
                        
                    break;
                   case "desfire":
                       // Implement DESFire-specific APDU commands and response processing
                       break;
                   default:
                       throw new Exception("Unsupported card type");
               }

            return data;

           /* // Process the response
            if (response.Length >= 2 && response[response.Length - 2] == (byte)0x90 && response[response.Length - 1] == (byte)0x00)
            {
                // Extract UID or other data based on card type
                string data =  ProcessResponse(response, cardType);
                return data;
            }
            else
            {
                throw new Exception("Failed to read card data");
            }
            */
          //  return "";
        }
        finally
        {
            // Disconnect from the reader
            reader?.Disconnect(SCardReaderDisposition.Unpower);
        }
            
    }

    private string ProcessResponse(byte[] response, string cardType)
    {
        switch (cardType.ToLower())
        {
            case "mifare classic":
                // Extract UID (assuming 4 bytes)
                string uid = BitConverter.ToString(response.Take(4).ToArray()).Replace("-", "");
                return uid;
            case "desfire":
                // Implement DESFire-specific response processing and data extraction
                return "";
                break;
            default:
                throw new Exception("Unsupported card type");
        }
    }
            
    public static void Main(string[] args)
    {
        bool Looping = true;
        while (Looping) { 
            var reader = new ACR122UReader();
            string cardType = "mifare classic"; // Replace with actual card type
            byte[] command = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 }; // Select Application (Mifare Classic)

            string data = reader.ReadCardUID(cardType, command);
            
            Console.WriteLine("\nCard Data: {0}", data);
            Console.WriteLine("Press R to read another key");
            ConsoleKeyInfo key = Console.ReadKey();
            if (!ConsoleKeyInfo.Equals(key.Key.ToString(), "R"))
            {
                Looping = false;
            }
        }
    }
}