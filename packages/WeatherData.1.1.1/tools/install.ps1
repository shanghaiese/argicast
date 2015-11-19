param($installPath, $toolsPath, $package, $project)
$file1 = $project.ProjectItems.Item("XSD").ProjectItems.Item("ForecastData.xsd")
$file2 = $project.ProjectItems.Item("XSD").ProjectItems.Item("InterpolatedData.xsd")
$file3 = $project.ProjectItems.Item("XSD").ProjectItems.Item("InterpolatedLTAData.xsd")
$file4 = $project.ProjectItems.Item("XSD").ProjectItems.Item("Locations.xsd")
$file5 = $project.ProjectItems.Item("XSD").ProjectItems.Item("Series.xsd")
 
 
# set 'Copy To Output Directory' to 'Copy always'
$copyToOutput1 = $file1.Properties.Item("CopyToOutputDirectory")
$copyToOutput1.Value = 1
 
$copyToOutput2 = $file2.Properties.Item("CopyToOutputDirectory")
$copyToOutput2.Value = 1

$copyToOutput3 = $file3.Properties.Item("CopyToOutputDirectory")
$copyToOutput3.Value = 1

$copyToOutput4 = $file4.Properties.Item("CopyToOutputDirectory")
$copyToOutput4.Value = 1

$copyToOutput5 = $file5.Properties.Item("CopyToOutputDirectory")
$copyToOutput5.Value = 1