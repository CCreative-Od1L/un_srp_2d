using UnityEngine;

namespace UnSrp2d.Shared.Debug
{
    /// <summary>
    /// Generic debug visualization for Collider2D bounds.
    /// Draws bounds in Scene view and optionally during runtime.
    /// </summary>
    public class Collider2DGizmos : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] Collider2D _collider;
        
        [Header("Display Options")]
        [SerializeField] bool _showInRuntime = true;
        [SerializeField] Color _fillColor = new Color(1f, 0.5f, 0f, 0.3f);
        [SerializeField] Color _outlineColor = new Color(1f, 0.5f, 0f, 1f);
        [SerializeField] bool _showCorners = true;

        void OnDrawGizmosSelected()
        {
            DrawBounds();
        }

        void OnDrawGizmos()
        {
            if (_showInRuntime && Application.isPlaying)
            {
                DrawBounds();
            }
        }

        void DrawBounds()
        {
            if (_collider == null)
                return;

            var bounds = _collider.bounds;

            Gizmos.color = _fillColor;
            Gizmos.DrawCube(bounds.center, bounds.size);

            Gizmos.color = _outlineColor;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            if (_showCorners)
                DrawCornerMarkers(bounds);
        }

        void DrawCornerMarkers(Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;
            var cornerSize = Mathf.Min(bounds.size.x, bounds.size.y) * 0.05f;

            Gizmos.color = _outlineColor;

            DrawCorner(new Vector3(min.x, min.y, 0), cornerSize);
            DrawCorner(new Vector3(max.x, min.y, 0), cornerSize);
            DrawCorner(new Vector3(min.x, max.y, 0), cornerSize);
            DrawCorner(new Vector3(max.x, max.y, 0), cornerSize);
        }

        void DrawCorner(Vector3 position, float size)
        {
            Gizmos.DrawLine(position + Vector3.left * size, position + Vector3.right * size);
            Gizmos.DrawLine(position + Vector3.down * size, position + Vector3.up * size);
        }
    }
}
