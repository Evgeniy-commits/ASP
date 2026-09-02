using System.Globalization;

namespace Blazor.Components.Pages
{
	public partial class Power
	{
		string baseValueString = "";
		string exponentValueString = "";

		double baseValue = 0;
		double exponentValue = 0;
		double ResultValue { get; set; }
		bool resultSet = false;

		string baseError = "";
		string exponentError = "";
		string resultError = "";

		void Calculate()
		{
			baseError = "";
			exponentError = "";
			resultError = "";
			resultSet = false;

			// ПАРСИНГ ЧИСЛЕЛ 

			if (!double.TryParse(baseValueString, NumberStyles.Any,
				CultureInfo.InvariantCulture, out baseValue))
			{
				baseError = "Некорректное число основания";
				return;
			}

			if (!double.TryParse(exponentValueString, NumberStyles.Any,
				CultureInfo.InvariantCulture, out exponentValue))
			{
				exponentError = "Некорректное число показателя";
				return;
			}
			// --------------------------------------

			//Валидация основания
			if (baseValue == 0 && exponentValue < 0)
			{
				resultError = "0 в отрицательной степени";
				return;
			}

			//Допустимый диапазон для экспоненты, чтобы не завис сервер
			if (exponentValue < -1000 || exponentValue > 1000)
			{
				exponentError = "Диапазон значений показателя [-1000; 1000]";
				return;
			}

			try
			{
				if (exponentValue >= 0)
				{
					ResultValue = Math.Pow(baseValue, exponentValue);
				}
				else
				{
					double positiveExp = -exponentValue;
					double denominator = Math.Pow(baseValue, positiveExp);

					if (denominator == 0 || double.IsInfinity(denominator))
					{
						resultError = "Деление на ноль";
						return;
					}

					ResultValue = 1 / denominator;
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
