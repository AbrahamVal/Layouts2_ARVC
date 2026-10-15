
using System;
using System.Globalization;

namespace Layouts2_ARVC
{
    public partial class MainPage : ContentPage
    {
        double n1, n2, resultado;
        string operacion = "";
        bool nuevoNumero = false;

        public MainPage()
        {
            InitializeComponent();
        }

        // Evento para los botones numéricos y el punto decimal
        private void OnNumberClicked(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            string numero = boton.Text;

            if (ResultLabel.Text == "0" || nuevoNumero)
            {
                ResultLabel.Text = "";
                nuevoNumero = false;
            }

            // Evita escribir más de un punto decimal
            if (numero == ".")
            {
                if (ResultLabel.Text.Contains("."))
                    return;

                if (ResultLabel.Text == "")
                    ResultLabel.Text = "0";
            }

            ResultLabel.Text += numero;
        }

        // Evento para los operadores matemáticos
        private void OnOperatorClicked(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            string operador = boton.Text;

            // Botón para borrar el último carácter
            if (operador == "⌫")
            {
                if (!nuevoNumero && ResultLabel.Text.Length > 0)
                {
                    ResultLabel.Text =
                        ResultLabel.Text.Length == 1
                        ? "0"
                        : ResultLabel.Text.Substring(
                            0, ResultLabel.Text.Length - 1);
                }

                return;
            }

            // Guarda el primer número y selecciona la operación
            if (double.TryParse(
                ResultLabel.Text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out n1))
            {
                operacion = operador;
                nuevoNumero = true;
            }
        }

        // Evento del botón igual
        private async void OnEqualsClicked(object sender, EventArgs e)
        {
            if (operacion == "")
                return;

            if (!double.TryParse(
                ResultLabel.Text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out n2))
                return;

            switch (operacion)
            {
                case "+":
                    resultado = n1 + n2;
                    break;

                case "-":
                    resultado = n1 - n2;
                    break;

                case "x":
                case "X":
                    resultado = n1 * n2;
                    break;

                case "÷":
                    if (n2 == 0)
                    {
                        await DisplayAlert(
                            "Error",
                            "No se puede dividir entre cero",
                            "OK");
                        return;
                    }

                    resultado = n1 / n2;
                    break;

                case "%":
                    resultado = n1 % n2;
                    break;

                default:
                    return;
            }

            ResultLabel.Text = resultado.ToString(
                "G",
                CultureInfo.InvariantCulture);

            nuevoNumero = true;
            operacion = "";
        }

        // Evento del botón C: limpia la calculadora
        private void OnClearClicked(object sender, EventArgs e)
        {
            ResultLabel.Text = "0";
            n1 = 0;
            n2 = 0;
            resultado = 0;
            operacion = "";
            nuevoNumero = false;
        }
    }
}
