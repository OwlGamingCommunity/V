public class PayDay
{
	public PayDay()
	{
		m_PayDayGUI = new CGUIPaydayOverview(() => { });

		NetworkEvents.ShowPayDayOverview += OnShowPayDayOverview;
		UIEvents.PaydayOverview_OnClose += OnClosePayDayOverview;
	}

	private void OnShowPayDayOverview(PayDayDetails paydayDetails)
	{
		m_PayDayGUI.SetVisible(true, true, false);

		float fTotalVehiclePayments = 0.0f;
		float fTotalVehicleTax = 0.0f;
		float fTotalPropertyPayments = 0.0f;
		float fTotalPropertyTax = 0.0f;
		// PAYCHECK TAB
		if (paydayDetails.m_lstPaycheckEntries.Count > 0)
		{
			foreach (CPaycheckEntry PayCheckEntry in paydayDetails.m_lstPaycheckEntries)
			{
				m_PayDayGUI.Add_PaycheckItem(PayCheckEntry.SalaryName, PayCheckEntry.NetIncome);
			}
		}
		else
		{
			m_PayDayGUI.SetNoPaychecks();
		}

		// VEHICLE & PROPERTIES TABS
		int numVehicles = 0;
		int numProperties = 0;
		foreach (PayDayVehicleOrPropertyDetails details in paydayDetails.m_VehicleAndPropertiesData)
		{
			if (details.IsVehicle)
			{
				if (!details.MissedPayment)
				{
					fTotalVehiclePayments += details.MonthlyPayment;
					fTotalVehicleTax += details.MonthlyTax;
				}

				m_PayDayGUI.Add_VehicleItem(details.DisplayName, details.MonthlyPayment, details.PaymentsRemaining, details.PaymentsMade, details.PaymentsMissed, details.MissedPayment, details.Reposessed, details.MonthlyTax);
				++numVehicles;
			}
			else
			{
				if (!details.MissedPayment)
				{
					fTotalPropertyPayments += details.MonthlyPayment;
					fTotalPropertyTax += details.MonthlyTax;
				}

				m_PayDayGUI.Add_PropertyItem(details.DisplayName, details.MonthlyPayment, details.PaymentsRemaining, details.PaymentsMade, details.PaymentsMissed, details.MissedPayment, details.Reposessed, details.MonthlyTax);
				++numProperties;
			}
		}

		if (numVehicles == 0)
		{
			m_PayDayGUI.SetNoVehicles();
		}

		if (numProperties == 0)
		{
			m_PayDayGUI.SetNoProperties();
		}

		// OVERVIEW TAB
		float fTotalStateIncomeTax = 0.0f;
		float fTotalFederalIncomeTax = 0.0f;
		float fTotalGrossIncome = 0.0f;
		float fTotalNetIncome = 0.0f;

		foreach (CPaycheckEntry PayCheckEntry in paydayDetails.m_lstPaycheckEntries)
		{
			PayCheckEntry.AppendValues(ref fTotalStateIncomeTax, ref fTotalFederalIncomeTax, ref fTotalGrossIncome, ref fTotalNetIncome);
		}

		m_PayDayGUI.Add_OverviewItem("Gross Income", fTotalGrossIncome);
		m_PayDayGUI.Add_OverviewItem("Net Income", fTotalNetIncome);
		m_PayDayGUI.Add_OverviewItem("State Income Tax", fTotalStateIncomeTax);
		m_PayDayGUI.Add_OverviewItem("Federal Income Tax", fTotalFederalIncomeTax);
		m_PayDayGUI.Add_OverviewItem("Donator Perk Income", paydayDetails.TotalDonatorPerks);
		m_PayDayGUI.Add_OverviewItem("Total Vehicle Payments", fTotalVehiclePayments);
		m_PayDayGUI.Add_OverviewItem("Total Vehicle Tax", fTotalVehicleTax);
		m_PayDayGUI.Add_OverviewItem("Total Property Payments", fTotalPropertyPayments);
		m_PayDayGUI.Add_OverviewItem("Total Property Tax", fTotalPropertyTax);
		m_PayDayGUI.Add_OverviewItem("Total Vehicle Tax Saved via Donation Perks", paydayDetails.TotalVehicleTaxSaved);
		m_PayDayGUI.Add_OverviewItem("Total Property Tax Saved via Donation Perks", paydayDetails.TotalPropertyTaxSaved);

		m_PayDayGUI.SetNumbers(paydayDetails.m_lstPaycheckEntries.Count, numVehicles, numProperties);
	}

	private void OnClosePayDayOverview()
	{
		m_PayDayGUI.SetVisible(false, false, false);
		m_PayDayGUI.Reload();
	}

	private CGUIPaydayOverview m_PayDayGUI = null;
}