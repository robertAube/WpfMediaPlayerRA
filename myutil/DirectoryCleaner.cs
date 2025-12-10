
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace MyUtil {

    public class DeleteResult {
        public int DeletedCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> DeletedFiles { get; } = new List<string>();
        public List<(string File, string Error)> Errors { get; } = new List<(string, string)>();
    }

    public static class DirectoryCleaner {
        /// <summary>
        /// Supprime les fichiers d'un répertoire avec gestion des erreurs et retries.
        /// </summary>
        /// <param name="directoryPath">Chemin du dossier.</param>
        /// <param name="includeSubdirectories">Inclure les fichiers des sous-dossiers.</param>
        /// <param name="maxRetries">Nombre maximum de tentatives par fichier.</param>
        /// <param name="initialDelayMs">Délai initial (ms) entre les tentatives.</param>
        /// <returns>Résultat détaillé des suppressions.</returns>
        public static DeleteResult DeleteAllFiles(
            string directoryPath,
            bool includeSubdirectories = false,
            int maxRetries = 3,
            int initialDelayMs = 100) {
            var result = new DeleteResult();

            if (string.IsNullOrWhiteSpace(directoryPath)) {
                result.Errors.Add(("", "Chemin de dossier vide ou invalide."));
                return result;
            }

            try {
                if (!Directory.Exists(directoryPath)) {
                    result.Errors.Add((directoryPath, "Le dossier n'existe pas."));
                    return result;
                }

                var option = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] files;

                try {
                    files = Directory.GetFiles(directoryPath, "*", option);
                }
                catch (UnauthorizedAccessException ex) {
                    result.Errors.Add((directoryPath, $"Accès refusé au dossier: {ex.Message}"));
                    return result;
                }
                catch (PathTooLongException ex) {
                    result.Errors.Add((directoryPath, $"Chemin trop long: {ex.Message}"));
                    return result;
                }

                foreach (var file in files) {
                    int attempt = 0;
                    int delay = initialDelayMs;

                    while (true) {
                        try {
                            // Facultatif: enlever l'attribut ReadOnly avant suppression
                            var attrs = File.GetAttributes(file);
                            if ((attrs & FileAttributes.ReadOnly) != 0) {
                                File.SetAttributes(file, attrs & ~FileAttributes.ReadOnly);
                            }

                            File.Delete(file);
                            result.DeletedCount++;
                            result.DeletedFiles.Add(file);
                            break; // réussi
                        }
                        catch (FileNotFoundException) {
                            // Déjà supprimé ou déplacé, on considère comme réussi
                            result.DeletedCount++;
                            result.DeletedFiles.Add(file + " (déjà absent)");
                            break;
                        }
                        catch (UnauthorizedAccessException ex) {
                            // Tentative supplémentaire possible si fichier verrouillé
                            if (++attempt <= maxRetries) {
                                Thread.Sleep(delay);
                                delay *= 2; // backoff exponentiel
                                continue;
                            }
                            result.ErrorCount++;
                            result.Errors.Add((file, $"Accès refusé après {attempt} tentatives: {ex.Message}"));
                            break;
                        }
                        //catch (IOException ex) {
                        //    // Peut provenir d'un verrou ou d'un partage de fichier
                        //    if (++attempt <= maxRetries) {
                        //        Thread.Sleep(delay);
                        //        delay *= 2;
                        //        continue;
                        //    }
                        //    result.ErrorCount++;
                        //    result.Errors.Add((file, $"IOException après {attempt} tentatives: {ex.Message}"));
                        //    break;
                        //}
                        catch (PathTooLongException ex) {
                            result.ErrorCount++;
                            result.Errors.Add((file, $"Chemin trop long: {ex.Message}"));
                            break;
                        }
                        catch (NotSupportedException ex) {
                            result.ErrorCount++;
                            result.Errors.Add((file, $"Chemin/format non supporté: {ex.Message}"));
                            break;
                        }
                        catch (Exception ex) {
                            result.ErrorCount++;
                            result.Errors.Add((file, $"Erreur inattendue: {ex.GetType().Name}: {ex.Message}"));
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                // Erreur globale inattendue
                result.Errors.Add((directoryPath, $"Erreur globale: {ex.GetType().Name}: {ex.Message}"));
            }

            return result;
        }
    }

    // --- Exemple d'utilisation ---
    //class Program {
    //    static void Main() {
    //        string cheminDossier = @"C:\Votre\Chemin\Ici";

    //        var res = DirectoryCleaner.DeleteAllFiles(
    //            cheminDossier,
    //            includeSubdirectories: false, // mettre true pour inclure les sous-dossiers
    //            maxRetries: 3,
    //            initialDelayMs: 100);

    //        Console.WriteLine($"Fichiers supprimés: {res.DeletedCount}");
    //        Console.WriteLine($"Erreurs: {res.ErrorCount}");

    //        if (res.Errors.Count > 0) {
    //            Console.WriteLine("\nDétails des erreurs:");
    //            foreach (var (file, error) in res.Errors) {
    //                Console.WriteLine($"- {file}: {error}");
    //            }
    //        }
    //    }
    //}
}