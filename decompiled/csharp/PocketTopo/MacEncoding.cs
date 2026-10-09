using System.Text;

namespace PocketTopo;

internal class MacEncoding : Encoding
{
	private const string special = "ÄÅÇÉÑÖÜáàâäãåçéèêëíìîïñóòôöõúùûü?°¢£§?¶ß®©?\u00b4\u00a8?ÆØ?±??¥µ?????ªº?æø¿¡¬????«»??ÀÃÕ????????÷?ÿ??€?????·???ÂÊÁËÈÍÎÏÌÓÔ?ÒÚÛÙ???\u00af???\u00b8???";

	public override int GetByteCount(char[] chars, int index, int count)
	{
		return count;
	}

	public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
	{
		for (int i = 0; i < charCount; i++)
		{
			int num = chars[charIndex++];
			if (num >= 128)
			{
				num = "ÄÅÇÉÑÖÜáàâäãåçéèêëíìîïñóòôöõúùûü?°¢£§?¶ß®©?\u00b4\u00a8?ÆØ?±??¥µ?????ªº?æø¿¡¬????«»??ÀÃÕ????????÷?ÿ??€?????·???ÂÊÁËÈÍÎÏÌÓÔ?ÒÚÛÙ???\u00af???\u00b8???".IndexOf((char)num);
				num = ((num < 0) ? 63 : (num + 128));
			}
			bytes[byteIndex++] = (byte)num;
		}
		return charCount;
	}

	public override int GetCharCount(byte[] bytes, int index, int count)
	{
		return count;
	}

	public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
	{
		for (int i = 0; i < byteCount; i++)
		{
			int num = bytes[byteIndex++];
			if (num >= 128)
			{
				num = "ÄÅÇÉÑÖÜáàâäãåçéèêëíìîïñóòôöõúùûü?°¢£§?¶ß®©?\u00b4\u00a8?ÆØ?±??¥µ?????ªº?æø¿¡¬????«»??ÀÃÕ????????÷?ÿ??€?????·???ÂÊÁËÈÍÎÏÌÓÔ?ÒÚÛÙ???\u00af???\u00b8???"[num - 128];
			}
			chars[charIndex++] = (char)num;
		}
		return byteCount;
	}

	public override int GetMaxByteCount(int charCount)
	{
		return charCount;
	}

	public override int GetMaxCharCount(int byteCount)
	{
		return byteCount;
	}
}
