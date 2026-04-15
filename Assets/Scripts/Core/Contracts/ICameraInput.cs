namespace UnSrp2d.Core.Contracts
{
    public interface ICameraInput
    {
        bool IsZoomModeActive { get; }
        bool IsZoomInPressed { get; }
        bool IsZoomOutPressed { get; }
    }
}