using UnityEngine;

public class Manager_SaveLoad : MonoBehaviour 
{
    #region Variables
    public static Manager_SaveLoad Instance { get; private set; }
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Debug.Log(Application.persistentDataPath);
    }

    public void Save(string filePath, string plainText) {
        if (string.IsNullOrEmpty(plainText)) {
            throw new ArgumentException("Plaintext cannot be null or empty.");
        }
        
        try {
            // Debug.Log($"Encrypting data: {plainText}");

            using Aes aes = Aes.Create();
            aes.Key = GetEncryptionKey();
            aes.GenerateIV(); // Ensure a new IV is generated

            // Save the IV first
            using FileStream dataStream = new(filePath, FileMode.Create);
            dataStream.Write(aes.IV, 0, aes.IV.Length);

            // Encrypt the data
            using CryptoStream cryptoStream = new(dataStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter writer = new(cryptoStream);

            writer.Write(plainText);
            Debug.Log("Data saved successfully!");
        }
        catch (Exception ex) {
            Debug.LogError($"Encryption error: {ex.Message}");
            Debug.LogError($"Stack Trace: {ex.StackTrace}");
        }
    }
    public string Load(string filePath) {
        if (!File.Exists(filePath)) {
            Debug.LogWarning("Load file not found!");
            return null;
        }

        try {
            using FileStream dataStream = new(filePath, FileMode.Open);
            byte[] iv = new byte[16];
            dataStream.Read(iv, 0, iv.Length);

            using Aes aes = Aes.Create();
            aes.Key = GetEncryptionKey();
            aes.IV = iv;

            using CryptoStream cryptoStream = new(dataStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);

            string decryptedData = reader.ReadToEnd();
            // Debug.Log($"Decrypted data successfully!\n{decryptedData}");

            return decryptedData;
        }
        catch (Exception ex) {
            Debug.LogError($"Load error: {ex.Message}");
            Debug.LogError($"Load stacktrace: {ex.StackTrace}");
            return null;
        }
    }   
    
    private byte[] GetEncryptionKey() {
        string keyString32 = "Th1sIsMy32ByteS3cur3K3y@ForAES25";
        // 32 bytes for AES-256
        byte[] keyBytes32 = Encoding.UTF8.GetBytes(keyString32);
        // Debug.Log($"Key length: {keyBytes32.Length}");

        return keyBytes32;
    }
}