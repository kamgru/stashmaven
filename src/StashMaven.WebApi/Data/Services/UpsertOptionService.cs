namespace StashMaven.WebApi.Data.Services;

[Injectable]
public class UpsertOptionService(StashMavenRepository repository)
{
    public async Task UpsertStashMavenOptionAsync(
        string key,
        string value)
    {
        StashMavenOption? option = await repository.GetStashMavenOptionAsync(key);
        
        if (option == null)
        {
            option = new StashMavenOption
            {
                Key = key,
                Value = value
            };
            repository.InsertStashMavenOption(option);
        }
        else
        {
            option.Value = value;
        }
    }

    public async Task UpsertCompanyOptionAsync(
        string key,
        string value)
    {
        CompanyOption? option = await repository.GetCompanyOptionAsync(key);
        
        if (option == null)
        {
            option = new CompanyOption
            {
                Key = key,
                Value = value
            };
            repository.InsertCompanyOption(option);
        }
        else
        {
            option.Value = value;
        }
    }
}
