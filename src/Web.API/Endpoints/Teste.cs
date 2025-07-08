using System.Collections.Generic;
using System;
using System.Linq;

namespace Problema
{

    public static class ServicoDeVeiculos
    {
        private static List<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
        public static void InsereVeiculo(Veiculo veiculo)
        {
            /*********************************
            **  Escreva seu código aqui...
            **  TODO: Implementar
            *********************************/
        }

        public static List<Veiculo> ObtenhaListaDeVeiculos()
        {
            /*********************************
            **  Escreva seu código aqui...
            **  TODO: Implementar
            *********************************/
            return Veiculos;
        }

        public static List<Veiculo> ObtenhaCarros()
        {
            /*********************************
            **  Escreva seu código aqui...
            **  TODO: Implementar
            *********************************/
            return Veiculos;
        }

        public static List<Veiculo> ObtenhaCaminhoes()
        {
            /*********************************
            **  Escreva seu código aqui...
            **  TODO: Implementar
            *********************************/
            return Veiculos;
        }
    }
    public class ControleVeicular
    {
        public static void Resolver(Veiculo[] entradas)
        {
            /************************************************************************
            **  IMPLEMENTE E USE O ServicoDeVeiculos para gerar a saída esperada.
            **************************************************************************/

            /************************************************************************
            **  Escreva seu código aqui...
            **  TODO: Implementar
            **************************************************************************/
            Console.Error.WriteLine("Debug Message, use Console.Error.WriteLine()");


            Console.Error.WriteLine("CARROS:{0},{1};CAMINHOES:{2},{3};",
                ServicoDeVeiculos.ObtenhaCarros().Count, string.Join(",", ServicoDeVeiculos.ObtenhaCarros().Select(s => s.Placa)), 
                ServicoDeVeiculos.ObtenhaCaminhoes().Count, string.Join(",", ServicoDeVeiculos.ObtenhaCaminhoes().Select(s => s.Placa))
                );
        }


/********************************************
**  NÃO ALTERE O CÓDIGO ABAIXO DESS LINHA ***
*********************************************/

        public static void Main(string[] args)
        {
            /************************************
            **  NÃO ALTERE O CÓDIGO DA MAIN() ***
            *************************************/
            try
            {
                int N = Convert.ToInt32(Console.ReadLine());
                Veiculo[] entradas = new Veiculo[N];

                for (int i = 0; i < N; i++)
                {
                    string[] dados = Console.ReadLine().Split(';');
                    entradas[i] = new Veiculo{
                        Codigo = Convert.ToInt32(dados[0]),
                        Placa = dados[1],
                        Tipo = (EnumTipo) Enum.Parse(typeof(EnumTipo), dados[2])
                    };
                }
                Resolver(entradas);
                Console.WriteLine("CARROS:{0},{1};CAMINHOES:{2},{3};",
                    ServicoDeVeiculos.ObtenhaCarros().Count, string.Join(",", ServicoDeVeiculos.ObtenhaCarros().Select(s => s.Placa)), 
                    ServicoDeVeiculos.ObtenhaCaminhoes().Count, string.Join(",", ServicoDeVeiculos.ObtenhaCaminhoes().Select(s => s.Placa))
                    );
            }
            catch (CodigoJaCadastradoException)
            {
                Console.WriteLine("CodigoJaCadastradoException");
            }
            catch (CadastroIrregularDePlacaException)
            {
                Console.WriteLine("CadastroIrregularDePlacaException");
            }
        }
    }

   /************************************
    **  NÃO ALTERE O CÓDIGO ABAIXO    ***
    *************************************/

    public class Veiculo
    {
        public int Codigo { get; set; }
        public string Placa { get; set; } = string.Empty;
        public EnumTipo Tipo { get; set; }
    }

    public enum EnumTipo
    {
        CARRO = 1,
        CAMINHAO = 2
    }
    public class CodigoJaCadastradoException : Exception
    {
        public CodigoJaCadastradoException() { }
        public CodigoJaCadastradoException(string message) : base(message) { }
        public CodigoJaCadastradoException(string message, Exception inner) : base(message, inner) { }
    }

    public class CadastroIrregularDePlacaException : Exception
    {
        public CadastroIrregularDePlacaException() { }
        public CadastroIrregularDePlacaException(string message) : base(message) { }
        public CadastroIrregularDePlacaException(string message, Exception inner) : base(message, inner) { }
    }
}
