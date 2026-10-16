
using System;
using System.Globalization;

namespace Layouts2_ARVC
{
    public partial class MainPage : ContentPage
    {
        private double n1 = 0;
        private double n2 = 0;
        private double resultado = 0;

        private string operacion = "";
        private string numeroActual = "0";
        private string expresion = "";

        private bool nuevoNumero = true;
        private bool hayOperacion = false;
        private bool resultadoMostrado = false;

        private readonly CultureInfo cultura =
            CultureInfo.InvariantCulture;

        public MainPage()
        {
            InitializeComponent();
            ActualizarPantalla();
        }

        // Botones numéricos y punto decimal
        private void OnNumberClicked(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            string numero = boton.Text;

            if (numero == ".")
            {
                if (!nuevoNumero && numeroActual.Contains("."))
                    return;

                if (nuevoNumero)
                {
                    numeroActual = "0.";
                    nuevoNumero = false;
                }
                else
                {
                    numeroActual += ".";
                }
            }
            else
            {
                if (nuevoNumero)
                {
                    numeroActual = numero;
                    nuevoNumero = false;
                }
                else
                {
                    if (numeroActual == "0")
                        numeroActual = numero;
                    else
                        numeroActual += numero;
                }
            }

            resultadoMostrado = false;
            ActualizarPantalla();
        }

        // Botones de operadores y borrar

        private async void OnOperatorClicked(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            string operador = boton.Text;

            // Botón para borrar
            if (operador == "⌫" || operador == "⌅")
            {
                if (resultadoMostrado)
                {
                    LimpiarCalculadora();
                    return;
                }

                if (!nuevoNumero && numeroActual.Length > 1)
                {
                    numeroActual = numeroActual.Substring(
                        0, numeroActual.Length - 1);
                }
                else
                {
                    numeroActual = "0";
                    nuevoNumero = true;
                }

                ActualizarPantalla();
                return;
            }

            if (!double.TryParse(
                numeroActual,
                NumberStyles.Float,
                cultura,
                out double valorActual))
            {
                return;
            }

            // Si acabamos de mostrar un resultado, continuar desde él
            if (resultadoMostrado)
            {
                expresion = numeroActual + " ";
                resultadoMostrado = false;
                hayOperacion = false;
                n1 = valorActual;
            }

            // Si ya hay una operación pendiente, resolverla
            if (hayOperacion && !nuevoNumero)
            {
                n2 = valorActual;

                if (!CalcularOperacion(out resultado))
                {
                    await DisplayAlert(
                        "Error",
                        "No se puede dividir entre cero.",
                        "OK");
                    return;
                }

                n1 = resultado;
                numeroActual = resultado.ToString("G", cultura);

                // Sustituir la expresión por la operación resuelta
                expresion = numeroActual + " ";
            }
            else if (!hayOperacion)
            {
                n1 = valorActual;
                expresion = numeroActual + " ";
            }

            // Seleccionar el operador nuevo sin duplicar el anterior
            operacion = operador;
            hayOperacion = true;
            nuevoNumero = true;

            expresion = expresion.TrimEnd() + " " + operador + " ";

            ActualizarPantalla();
        }

        // Botón igual
        private async void OnEqualsClicked(object sender, EventArgs e)
        {
            if (!hayOperacion || nuevoNumero)
                return;

            if (!double.TryParse(
                numeroActual,
                NumberStyles.Float,
                cultura,
                out n2))
            {
                return;
            }

            if (!CalcularOperacion(out resultado))
            {
                await DisplayAlert(
                    "Error",
                    "No se puede dividir entre cero.",
                    "OK");

                return;
            }

            expresion += FormatearNumero(n2) + " =";

            numeroActual = resultado.ToString("G", cultura);

            n1 = resultado;
            operacion = "";
            hayOperacion = false;
            nuevoNumero = true;
            resultadoMostrado = true;

            ActualizarPantalla();
        }

        // Realizar la operación matemática
        private bool CalcularOperacion(out double valor)
        {
            valor = 0;

            switch (operacion)
            {
                case "+":
                    valor = n1 + n2;
                    break;

                case "-":
                    valor = n1 - n2;
                    break;

                case "x":
                case "X":
                case "*":
                case "×":
                    valor = n1 * n2;
                    break;

                case "÷":
                case "/":
                    if (n2 == 0)
                        return false;

                    valor = n1 / n2;
                    break;

                case "%":
                    if (n2 == 0)
                        return false;

                    valor = n1 % n2;
                    break;

                default:
                    return false;
            }

            return !double.IsNaN(valor) &&
                   !double.IsInfinity(valor);
        }

        // Mostrar procedimiento y resultado
        private void ActualizarPantalla()
        {
            ExpressionLabel.Text = expresion;

            if (numeroActual == "")
                numeroActual = "0";

            ResultLabel.Text = numeroActual;
        }

        // Formatear números sin decimales innecesarios
        private string FormatearNumero(double numero)
        {
            return numero.ToString("G", cultura);
        }

        // Botón C: limpiar todo
        private void OnClearClicked(object sender, EventArgs e)
        {
            LimpiarCalculadora();
        }

        private void LimpiarCalculadora()
        {
            n1 = 0;
            n2 = 0;
            resultado = 0;

            operacion = "";
            numeroActual = "0";
            expresion = "";

            nuevoNumero = true;
            hayOperacion = false;
            resultadoMostrado = false;

            ActualizarPantalla();
        }
    }
}
