using System.Security.Cryptography.X509Certificates;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;

public class CertificateHandlerPublicKey : CertificateHandler
{
    // Define the public key of the SSL certificate you're expecting
    private static string PUB_KEY = "MIIDtTCCAzugAwIBAgISA4YU4jq4FTf5K135547NXUV+MAoGCCqGSM49BAMDMDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQDEwJFNTAeFw0yNDA5MjAxNjMwNTJaFw0yNDEyMTkxNjMwNTFaMCYxJDAiBgNVBAMTG3d3dy5zaWRlc2Nyb2xsZXJnYW1lLm9ubGluZTB2MBAGByqGSM49AgEGBSuBBAAiA2IABFjJVqwEuzpjc1+ezn4E6k+y0plMn/qQIhmm9ny8Wa9nGYkZSuFIq6/XaUnrqZAqTbqGuy2TK+8ok5BRnmTKQnfUrtfmylE2lrdCk4Eq8t0c3Fiz7SJItn2wo/AiTJRB6KOCAh4wggIaMA4GA1UdDwEB/wQEAwIHgDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwDAYDVR0TAQH/BAIwADAdBgNVHQ4EFgQUOe4LXnZKFSRCfow0SOK0q3BohyUwHwYDVR0jBBgwFoAUnytfzzwhT50Et+0rLMTGcIvS1w0wVQYIKwYBBQUHAQEESTBHMCEGCCsGAQUFBzABhhVodHRwOi8vZTUuby5sZW5jci5vcmcwIgYIKwYBBQUHMAKGFmh0dHA6Ly9lNS5pLmxlbmNyLm9yZy8wJgYDVR0RBB8wHYIbd3d3LnNpZGVzY3JvbGxlcmdhbWUub25saW5lMBMGA1UdIAQMMAowCAYGZ4EMAQIBMIIBBQYKKwYBBAHWeQIEAgSB9gSB8wDxAHYASLDja9qmRzQP5WoC+p0w6xxSActW3SyB2bu/qznYhHMAAAGSEHs0JQAABAMARzBFAiEA7tNGMqRm+h8q6LGZcpx7X2WX0VUdLqhv8rzlI+b83LoCICQJb/H4PFSrJAAFtgC/8fFRL9FKoLpUr5OeTD9wa6/lAHcA7s3QZNXbGs7FXLedtM0TojKHRny87N7DUUhZRnEftZsAAAGSEHs0CAAABAMASDBGAiEA5YVoAQlHx3dYGud0VuPYRG7FDOR0jTQpmts7nGeDkTsCIQD7pUppJ+FiTPkVBxxT0iChbRcNgZ3P82q5Dq9pkoBd/DAKBggqhkjOPQQDAwNoADBlAjEAwK2T/QO6EYwp0UxCz36FmV77GtQfFVL/lu4Qh0ZmLUdB4yNAqTl9/xr/7K7g8tN0AjAwUs+OzxlMXmHPy1KAG/h983LZzZsbpHTStFTTuYRAXrMfP6g6JlhJjwR6dVSwFxk=";

    // Override the ValidateCertificate method
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        X509Certificate2 certificate = new X509Certificate2(certificateData);
        string pk = certificate.GetPublicKeyString();

        Debug.Log("Server Public Key: " + pk); // This is useful for debugging

        // Compare the public key of the server certificate with the predefined one
        if (pk.ToLower().Equals(PUB_KEY.ToLower()))
        {
            return true; // Certificate is valid
        }

        return false; // Certificate is invalid
    }
}
