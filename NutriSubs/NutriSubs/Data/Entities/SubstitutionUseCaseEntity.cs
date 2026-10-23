using SQLite;

namespace NutriSubs.Data.Entities
{
    // Many-to-many link between a Substitution and the SubstitutionUseCase(s) it is suitable for.
    [Table("SubstitutionUseCases")]
    public class SubstitutionUseCaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Indexed]
        public int SubstitutionId { get; set; }

        [NotNull]
        public SubstitutionUseCase UseCase { get; set; }
    }
}
