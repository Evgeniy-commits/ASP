using System.Numerics;

namespace Blazor.Components.Pages
{
	public partial class Fibonachy
	{
		int nValue = 0;
		BigInteger ResultValue { get; set; }

		bool resultSet = false;

		string resultError = "";
		string seqDisplay = "";

		void Calculate()
		{
			resultError = "";
			resultSet = false;
			seqDisplay = "";

			if (nValue < 0)
			{
				resultError = "Номер должен быть положительным";
				return;
			}

			try
			{
				if (nValue == 0)
				{
					ResultValue = 0;
					seqDisplay = "0";
				}
				else if (nValue == 1)
				{
					ResultValue = 1;
					seqDisplay = "0, 1";
				}
				else
				{
					BigInteger a = 0;
					BigInteger b = 1;
					BigInteger cur = 0;

					List<BigInteger> list = new List<BigInteger> { 0, 1 };

					for (int i = 2; i <= nValue; i++)
					{
						cur = a + b;
						a = b;
						b = cur;
						list.Add(cur);
					}

					ResultValue = cur;

					seqDisplay = string.Join(", ", list);
				}

				resultSet = true;
			}
			catch (Exception ex)
			{
				resultError = $"Error: {ex.Message}";
			}
		}
	}
}
