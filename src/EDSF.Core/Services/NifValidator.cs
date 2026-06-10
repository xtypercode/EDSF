namespace EDSF.Core.Services;

public static class NifValidator
{
    public static bool IsValid(string? nif)
    {
        if (string.IsNullOrWhiteSpace(nif)) return false;
        nif = nif.Trim();
        if (nif.Length != 10) return false;
        if (!nif.All(char.IsDigit)) return false;

        var total = 0;
        for (var i = 0; i < 9; i++)
            total += (nif[i] - '0') * (10 - i);

        var remainder = total % 11;
        var checkDigit = remainder < 2 ? 0 : 11 - remainder;

        return checkDigit == nif[9] - '0';
    }

    public static bool IsCompany(string nif)
    {
        if (string.IsNullOrWhiteSpace(nif) || nif.Length < 1) return false;
        var first = nif[0];
        return first is '5' or '6' or '7' or '8' or '9';
    }
}
