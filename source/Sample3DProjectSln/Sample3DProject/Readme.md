# Refrence Matter

While developing the GenFusionsRevitCore.Servers3dContext I need to run it from the sample project easily. Everytime
I make a change I should not be creating releases or packages. That is why Sample3DProject references
Servers3dContext based on configuration.

If the configuration of the solution contains "Debug" the projects directly references the csproj so I can do realtime
changes:
```csproj
<ItemGroup>
	<ProjectReference Include="..\..\GenFusionsRevitCore.Servers3dContext\GenFusionsRevitCore.Servers3dContext.csproj" Condition="$(Configuration.Contains('Debug'))"/>
</ItemGroup>
```

If the configuration of the solution contains "Release" then it references the Servers3dContext from Nuget package on Github:
```csproj

<ItemGroup>
	<PackageReference Include="GenFusionsRevitCore.Servers3dContext" Version="$(RevitVersion).*" Condition="$(Configuration.Contains('Release'))" />
</ItemGroup>

<ItemGroup>
	<ProjectReference Include="..\..\GenFusionsRevitCore.Servers3dContext\GenFusionsRevitCore.Servers3dContext.csproj" Condition="$(Configuration.Contains('Debug'))"/>
</ItemGroup>
```