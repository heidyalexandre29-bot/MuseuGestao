using System;

namespace MuseuGestao
{
    public class Visitante
    {
        private static int _contadorId = 1;

        public int Id { get; }
        public string Nome { get; private set; }
        public string Contacto { get; private set; }
        public CategoriaBilhete Categoria { get; private set; }

        public Visitante(string nome, string contacto, CategoriaBilhete categoria)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do visitante não pode estar vazio.");
            if (string.IsNullOrWhiteSpace(contacto))
                throw new ArgumentException("O contacto não pode estar vazio.");

            Id = _contadorId++;
            Nome = nome;
            Contacto = contacto;
            Categoria = categoria;
        }

        public override string ToString()
        {
            return $"[Visitante #{Id}] {Nome} | {Categoria} | Contacto: {Contacto}";
        }
    }
}
