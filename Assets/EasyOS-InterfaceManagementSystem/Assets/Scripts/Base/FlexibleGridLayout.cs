using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Base
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns,
            WidthAndHeight
        }

        public enum Alignment
        {
            Horizontal = 0,
            Vertical = 1
        }
        
        public FitType fitType;
        public Alignment alignment;

        public int columns;
        public int rows;
        public Vector2 cellSize;
        public Vector2 spacing;

        public bool fitX;
        public bool fitY;
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform )
            {
                fitX = true;
                fitY = true;
                float sqrRt = Mathf.Sqrt(transform.childCount);
                columns = Mathf.CeilToInt(sqrRt);
                rows = Mathf.CeilToInt(sqrRt);
            }
            
            switch (fitType)
            {
                case FitType.Width:
                case FitType.FixedColumns:
                    columns = Mathf.CeilToInt(transform.childCount / (float) rows);
                    break;
                case FitType.Height:
                case FitType.FixedRows:
                    rows = Mathf.CeilToInt(transform.childCount / (float) columns);
                    break;
                case FitType.WidthAndHeight:
                    break;
            }

            Rect rect = rectTransform.rect;

            float parentWidth = 0;
            float parentHeight = 0;  
            
            switch (alignment)
            {
                case Alignment.Horizontal:
                    parentWidth = rect.width;
                    parentHeight = rect.height;
                    break;
                case Alignment.Vertical:
                    parentWidth = rect.height;
                    parentHeight = rect.width;
                    break;
            }

            float cellWidth = (parentWidth / (float)rows) - (spacing.x) - (padding.left / (float)rows) - (padding.right / (float)rows);
            float cellHeight = (parentHeight / (float)columns) - (spacing.y) - (padding.top / (float)columns) - (padding.bottom / (float)columns);



            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                int rowCount = i / rows;
                int columnCount = i % rows;

                RectTransform item = rectChildren[i];
                
                float xPos = (cellSize.x * columnCount) + (spacing.x * columnCount + spacing.x/2) + padding.left;
                float yPos = (cellSize.y * rowCount) + (spacing.y * rowCount + spacing.y/2) + padding.top;
                
                switch (alignment)
                {
                    case Alignment.Horizontal:
                        SetChildAlongAxis(item, 0, xPos, cellSize.x);
                        SetChildAlongAxis(item, 1, yPos, cellSize.y);
                        break;
                    case Alignment.Vertical:
                        SetChildAlongAxis(item, 1, xPos, cellSize.x);
                        SetChildAlongAxis(item, 0, yPos, cellSize.y);
                        break;
                }
            }

        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}
