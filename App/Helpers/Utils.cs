/*prompt: Add a new button to export the data to JSON. There should be two modes:
o Unfiltered export (exports the entire data)
o Filtered export (exports only the currently filtered rows)
• Also add the ability to select specific columns for export:
o If no column is selected, export all columns.
o If certain columns (e.g., 1st and 4th) are selected, export only those columns.
o The selected columns should visually change color to indicate selection.
o The exported JSON should contain only the selected column data.
4. Utility Class for JSON Export
• Create a new C# class file named Utils.cs.
• Inside it, implement a generic method that can export any class to JSON.
• The method should work with any model class.
• This class must be implemented as a singleton, so it can be accessed from anywhere in the
project.
5. Folder Structure Reminder (MVP)
Since your project follows the MVP structure in a Razor Pages application:
• Place the ClassInformationTable and related data models in the Models folder.
• Place the Utils.cs class in a separate folder called Helpers or Utilities.
• Place pagination logic, filtering logic, and UI-related code in the appropriate Pages folder*/

public class Utils
{
    private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());

    private Utils() { }

    public static Utils Instance => _instance.Value;

    public string ExportToJson<T>(IEnumerable<T> data, IEnumerable<string> selectedColumns)
    {
        var filteredData = data.Select(item =>
        {
            var dict = new Dictionary<string, object>();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                if (!selectedColumns.Any() || selectedColumns.Contains(prop.Name))
                {
                    dict[prop.Name] = prop.GetValue(item);
                }
            }

            return dict;
        });

        return System.Text.Json.JsonSerializer.Serialize(filteredData);
    }
}

