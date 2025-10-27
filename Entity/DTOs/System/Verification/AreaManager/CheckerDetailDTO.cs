using Entity.DTOs.System.Inventary.AreaManager.InventoryDetail;

namespace Entity.DTOs.System.Verification.AreaManager
{
    public class CheckerDetailDTO
    {
        public int Id { get; set; }
        public UserInfoDTO User { get; set; } = null!;
        public BranchInfoDTO Branch { get; set; } = null!;
    }
}
