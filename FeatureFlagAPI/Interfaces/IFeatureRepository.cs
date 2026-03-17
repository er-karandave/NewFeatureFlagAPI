using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IFeatureRepository
    {
        Task<IEnumerable<FeatureModel>> GetAllFeaturesAsync();
        Task<FeatureModel?> GetFeatureByIdAsync(int id);
        Task<int> CreateFeatureAsync(FeatureModel feature);
        Task<bool> UpdateFeatureAsync(FeatureModel feature);
        Task<bool> DeleteFeatureAsync(int id);
    }
}
