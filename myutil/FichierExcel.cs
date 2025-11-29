using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtil {
    internal class FichierExcel {
        public static List<string> lireColonneExcel(string filePath, int noLigneDebut = 1, int noColonne = 1, string nomFeuille = "") { // A2 correspond à noColonne 1, noLigne 2
            List<string> listeValeur = new List<string>();
            int noFeuille = 1; //par défaut

            // Ouvrir le classeur


            using (var workbook = new XLWorkbook(filePath)) { //Install-Package ClosedXML
                if (nomFeuille != "") {
                    noFeuille = GetSheetIndexByName(workbook, nomFeuille);
                }

                // Sélectionner la première feuille
                var worksheet = workbook.Worksheet(noFeuille);

                int row = noLigneDebut; // A1 correspond à ligne 1

                // Boucle tant que la cellule n'est pas vide
                while (!worksheet.Cell(row, noColonne).IsEmpty()) {
                    string value = worksheet.Cell(row, noColonne).GetString();
                    listeValeur.Add(value);
                    //Console.WriteLine($"A{row} : {value}");
                    row++;
                }
            }
            return listeValeur;
        }

        public static string lireCelluleExcel(string filePath, int noLigne = 1, int noColonne = 1, string nomFeuille = "") { // A1 correspond à noLigne 1, noColonne 1
            string valeur = "";
            int noFeuille = 1; //par défaut

            // Ouvrir le classeur
            using (var workbook = new XLWorkbook(filePath)) { //Install-Package ClosedXML
                if (nomFeuille != "") {
                    noFeuille = GetSheetIndexByName(workbook, nomFeuille);
                }
                // Sélectionner la première feuille
                var worksheet = workbook.Worksheet(noFeuille);

                valeur = worksheet.Cell(noLigne, noColonne).GetString();
            }
            return valeur;
        }

        /// <summary>
        /// Retourne l'index (1-based) d'une feuille dans le classeur à partir de son nom.
        /// Retourne -1 si la feuille n'existe pas.
        /// </summary>
        public static int GetSheetIndexByName(XLWorkbook workbook, string sheetName) {
            if (workbook == null) throw new ArgumentNullException(nameof(workbook));
            if (string.IsNullOrWhiteSpace(sheetName)) throw new ArgumentException("Le nom de la feuille ne peut pas être vide.", nameof(sheetName));

            int index = 0;
            foreach (var ws in workbook.Worksheets) {
                index++;
                if (ws.Name.Equals(sheetName, StringComparison.OrdinalIgnoreCase)) {
                    return index;
                }
            }
            return -1; // Feuille non trouvée
        }

    }
}

//        public static List<string> lireColonneExcelEPPlus(string filePath, int noLigneDebut = 1, int noColonne = 1) { // A1 correspond à noLigne 1, noColonne 1
//            // Install-Package EPPlus
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//            List<string> listeValeur = new List<string>();

//            // Ouvrir le fichier
//            using (var package = new ExcelPackage(new FileInfo(filePath))) {
//                var worksheet = package.Workbook.Worksheets[0];

//                int row = noLigneDebut; 
//                while (!string.IsNullOrEmpty(worksheet.Cells[row, noColonne].Text)) // Colonne A = index 1
//                {
//                    string value = worksheet.Cells[row, 1].Text;
////                    Console.WriteLine($"A{row} : {value}");
//                    row++;
//                }
//            }

//            return listeValeur;
//        }

