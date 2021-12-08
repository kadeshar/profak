﻿using ProFak.DB;
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
	partial class KontrahentEdytor : KontrahentEdytorBase
	{
		public KontrahentEdytor()
		{
			InitializeComponent();

			kontroler.Slownik(comboBoxStan, "archiwalny", "aktywny");

			kontroler.Powiazanie(textBoxNazwa, kontrahent => kontrahent.Nazwa);
			kontroler.Powiazanie(textBoxPelnaNazwa, kontrahent => kontrahent.PelnaNazwa);
			kontroler.Powiazanie(textBoxNIP, kontrahent => kontrahent.NIP);
			kontroler.Powiazanie(textBoxAdresRejestrowy, kontrahent => kontrahent.AdresRejestrowy);
			kontroler.Powiazanie(textBoxAdresKorespondencyjny, kontrahent => kontrahent.AdresKorespondencyjny);
			kontroler.Powiazanie(textBoxTelefon, kontrahent => kontrahent.Telefon);
			kontroler.Powiazanie(textBoxEMail, kontrahent => kontrahent.EMail);
			kontroler.Powiazanie(textBoxRachunekBankowy, kontrahent => kontrahent.RachunekBankowy);
			kontroler.Powiazanie(textBoxUwagi, kontrahent => kontrahent.Uwagi);
			kontroler.Powiazanie(comboBoxStan, kontrahent => kontrahent.CzyArchiwalny);

			Wymagane(textBoxNazwa);
		}

		private void textBoxNazwa_TextChanged(object sender, EventArgs e)
		{
			textBoxPelnaNazwa.PlaceholderText = textBoxNazwa.Text;
		}

		private void textBoxAdresRejestrowy_TextChanged(object sender, EventArgs e)
		{
			textBoxAdresKorespondencyjny.PlaceholderText = textBoxAdresRejestrowy.Text;
		}

		public override void KoniecEdycji()
		{
			base.KoniecEdycji();
			if (String.IsNullOrEmpty(Rekord.PelnaNazwa)) Rekord.PelnaNazwa = Rekord.Nazwa;
			if (String.IsNullOrEmpty(Rekord.AdresKorespondencyjny)) Rekord.AdresKorespondencyjny = Rekord.AdresRejestrowy;
		}
	}

	class KontrahentEdytorBase : Edytor<Kontrahent>
	{
	}
}
