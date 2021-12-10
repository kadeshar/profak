﻿using ProFak.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProFak.UI
{
	class DodajWplateAkcja : AkcjaNaSpisie<Faktura>
	{
		public override string Nazwa => "💰 Dodaj wpłatę [ALT-W]";
		public override bool CzyDostepnaDlaRekordow(IEnumerable<Faktura> zaznaczoneRekordy) => zaznaczoneRekordy.Count() == 1 && !zaznaczoneRekordy.Single().CzyZaplacona;
		public override bool CzyKlawiszSkrotu(Keys klawisz, Keys modyfikatory) => klawisz == Keys.W && modyfikatory == Keys.Control;

		public override void Uruchom(Kontekst kontekst, ref IEnumerable<Faktura> zaznaczoneRekordy)
		{
			using var nowyKontekst = new Kontekst(kontekst);
			using var transakcja = nowyKontekst.Transakcja();
			var faktura = zaznaczoneRekordy.Single();
			var wplata = new Wplata { FakturaRef = faktura, Kwota = faktura.PozostaloDoZaplaty, Data = DateTime.Now.Date };
			kontekst.Baza.Zapisz(wplata);
			using var edytor = new WplataEdytor();
			using var okno = new Dialog("Nowa pozycja", edytor, nowyKontekst);
			edytor.Przygotuj(nowyKontekst, wplata);
			if (okno.ShowDialog() != DialogResult.OK) return;
			edytor.KoniecEdycji();
			kontekst.Baza.Zapisz(wplata);
			transakcja.Zatwierdz();
		}
	}
}
