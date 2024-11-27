﻿using Microsoft.EntityFrameworkCore;
using ProFak.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProFak.UI
{
	partial class PozycjaFakturyEdytor : PozycjaFakturyEdytorBase
	{
		private Slownik<Towar> slownikTowarow;

		public PozycjaFakturyEdytor()
		{
			InitializeComponent();

			kontroler.Slownik<decimal?>(comboBoxStawkaRyczaltu, null, 17m, 15m, 14m, 12.5m, 12m, 10m, 8.5m, 5.5m, 3m, 2m);

			kontroler.Powiazanie(numericUpDownLP, pozycja => pozycja.LP);
			kontroler.Powiazanie(comboBoxTowar, pozycja => pozycja.Opis);
			kontroler.Powiazanie(numericUpDownIlosc, pozycja => pozycja.Ilosc, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownCenaNetto, pozycja => pozycja.CenaNetto, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownCenaVat, pozycja => pozycja.CenaVat, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownCenaBrutto, pozycja => pozycja.CenaBrutto, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownWartoscNetto, pozycja => pozycja.WartoscNetto, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownWartoscVat, pozycja => pozycja.WartoscVat, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownWartoscBrutto, pozycja => pozycja.WartoscBrutto, PrzeliczCeny);
			kontroler.Powiazanie(checkBoxWedlugBrutto, pozycja => pozycja.CzyWedlugCenBrutto, KonfigurujCeny);
			kontroler.Powiazanie(checkBoxRecznie, pozycja => pozycja.CzyWartosciReczne, KonfigurujCeny);
			kontroler.Powiazanie(comboBoxStawkaVat, pozycja => pozycja.StawkaVatRef);
			kontroler.Powiazanie(comboBoxGTU, pozycja => pozycja.GTU);
			kontroler.Powiazanie(comboBoxStawkaRyczaltu, pozycja => pozycja.StawkaRyczaltu);
			kontroler.Powiazanie(numericUpDownRabatProcent, pozycja => pozycja.RabatProcent, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownRabatCena, pozycja => pozycja.RabatCena, PrzeliczCeny);
			kontroler.Powiazanie(numericUpDownRabatWartosc, pozycja => pozycja.RabatWartosc, PrzeliczCeny);

			Wymagane(comboBoxTowar);
			Wymagane(comboBoxStawkaVat);
		}

		protected override void KontekstGotowy()
		{
			base.KontekstGotowy();

			slownikTowarow = new Slownik<Towar>(
				Kontekst, comboBoxTowar, buttonTowar,
				Kontekst.Baza.Towary.OrderBy(towar => towar.Nazwa).ToList,
				towar => towar.Nazwa,
				UstawTowar,
				Spisy.Towary);
			slownikTowarow.Zainstaluj();

			new Slownik<StawkaVat>(
				Kontekst, comboBoxStawkaVat, buttonStawkaVat,
				Kontekst.Baza.StawkiVat.OrderBy(stawka => stawka.Skrot).ToList,
				stawka => stawka.Skrot,
				stawka => { PrzeliczCeny(); },
				Spisy.StawkiVat)
				.Zainstaluj();
		}

		protected override void RekordGotowy()
		{
			base.RekordGotowy();
			KonfigurujPoleIlosci();
			KonfigurujCeny();
		}

		private void KonfigurujPoleIlosci()
		{
			var towar = Kontekst.Baza.Towary.Include(towar => towar.JednostkaMiary).FirstOrDefault(towar => towar.Id == Rekord.TowarId);
			if (towar == null)
			{
				labelJednostka.Text = "";
				numericUpDownIlosc.DecimalPlaces = 0;
			}
			else
			{
				labelJednostka.Text = towar.JednostkaMiary.Nazwa;
				numericUpDownIlosc.DecimalPlaces = towar.JednostkaMiary.LiczbaMiescPoPrzecinku;
			}
		}

		private void KonfigurujCeny()
		{
			numericUpDownCenaNetto.Enabled = Rekord.CzyWartosciReczne || !Rekord.CzyWedlugCenBrutto;
			numericUpDownCenaVat.Enabled = Rekord.CzyWartosciReczne;
			numericUpDownCenaBrutto.Enabled = Rekord.CzyWartosciReczne || Rekord.CzyWedlugCenBrutto;
			numericUpDownWartoscNetto.Enabled = Rekord.CzyWartosciReczne;
			numericUpDownWartoscVat.Enabled = Rekord.CzyWartosciReczne;
			numericUpDownWartoscBrutto.Enabled = Rekord.CzyWartosciReczne;
		}

		private void PrzeliczCeny()
		{
			Rekord.PrzeliczCeny(Kontekst.Baza);
			kontroler.AktualizujKontrolki();
		}

		private void UstawTowar(Towar towar)
		{
			if (towar == null || Rekord.TowarRef == towar.Ref) return;
			Rekord.TowarRef = towar;
			Rekord.Opis = towar.Nazwa;
			Rekord.CzyWedlugCenBrutto = towar.CzyWedlugCenBrutto;
			Rekord.CenaBrutto = towar.CenaBrutto;
			Rekord.CenaNetto = towar.CenaNetto;
			Rekord.StawkaVatRef = towar.StawkaVatRef;
			Rekord.GTU = towar.GTU;
			Rekord.StawkaRyczaltu = towar.StawkaRyczaltu;
			KonfigurujPoleIlosci();
			KonfigurujCeny();
			PrzeliczCeny();
		}

		private void buttonNowyTowar_Click(object sender, EventArgs e)
		{
			var towar = new Towar { Nazwa = comboBoxTowar.Text, StawkaVatRef = Rekord.StawkaVatRef, CenaNetto = Rekord.CenaNetto, CenaBrutto = Rekord.CenaBrutto, GTU = Rekord.GTU, CzyWedlugCenBrutto = Rekord.CzyWedlugCenBrutto };
			using var nowyKontekst = new Kontekst(Kontekst);
			using var transakcja = nowyKontekst.Transakcja();
			nowyKontekst.Dodaj(towar);
			nowyKontekst.Baza.Zapisz(towar);
			using var edytor = new TowarEdytor();
			using var okno = new Dialog("Nowy towar", edytor, nowyKontekst);
			edytor.Przygotuj(nowyKontekst, towar);
			if (okno.ShowDialog() != DialogResult.OK) return;
			edytor.KoniecEdycji();
			nowyKontekst.Baza.Zapisz(towar);
			transakcja.Zatwierdz();
			slownikTowarow.Przeladuj();
			UstawTowar(towar);
		}
	}

	class PozycjaFakturyEdytorBase : Edytor<PozycjaFaktury>
	{
	}
}
