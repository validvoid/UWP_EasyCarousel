﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Marduk.Controls
{
    public sealed class EasyCarousel : Panel
    {
        private DataTemplate _itemTemplate;

        private RectangleGeometry _viewportRect;

        private Compositor _carouselCompositor;

        public event EventHandler<FrameworkElement> ItemTapped;

        #region Properties

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(EasyCarousel), new PropertyMetadata(0, OnSelectedIndexChanged));

        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(EasyCarousel), new PropertyMetadata(300d, null));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(EasyCarousel), new PropertyMetadata(310d, null));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(EasyCarousel), new PropertyMetadata(150d, null));

        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(EasyCarousel), new PropertyMetadata(null, OnItemsSourceChanged));

        public double ItemSpacing
        {
            get { return (double)GetValue(ItemSpacingProperty); }
            set { SetValue(ItemSpacingProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set { _itemTemplate = value; }
        }

        public static readonly DependencyProperty ItemSpacingProperty =
            DependencyProperty.Register("ItemSpacing", typeof(double), typeof(EasyCarousel), new PropertyMetadata(0, null));

        #endregion


        public EasyCarousel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            Visual carouselVisual = ElementCompositionPreview.GetElementVisual(this);
            _carouselCompositor = carouselVisual.Compositor;

            this.Tapped += OnTapped;

        }

        #region Event handlers

        private void OnTapped(object sender, TappedRoutedEventArgs args)
        {
            FrameworkElement fxElement = args.OriginalSource as FrameworkElement;
            ItemTapped(sender, fxElement);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            if (e.NewValue == e.OldValue)
                return;

            EasyCarousel instance = d as EasyCarousel;

            if (instance == null)
                return;

            if (DesignMode.DesignModeEnabled)
                return;

            instance.ShiftItemsAnimatedly((int)e.NewValue);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            if (e.NewValue == e.OldValue)
                return;

            EasyCarousel instance = d as EasyCarousel;

            if (instance == null)
                return;

            instance.BindItems();
        }

        #endregion


        private void BindItems()
        {
            if (ItemsSource == null || (ItemsSource as IEnumerable) == null)
                return;

            this.Children.Clear();

            foreach (object item in (IEnumerable)ItemsSource)
                this.CreateItem(item);

        }

        private void CreateItem(object item)
        {
            FrameworkElement element;

            if (ItemTemplate != null)
            {
                element = ItemTemplate.LoadContent() as FrameworkElement;
            }
            else
            {
                element = new TextBlock();
            }

            if (element == null)
                return;

            element.DataContext = item;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            this.Children.Add(element);
        }

        /// <summary>
        /// Shift items with composition-powered animations.
        /// </summary>
        /// <param name="targetIndex"></param>
        private void ShiftItemsAnimatedly(int targetIndex)
        {
            if (targetIndex < 0 || this.Children.Count <= 0)
                return;

            int sliceLength = (int)Math.Ceiling((double)this.Children.Count / 2);

            int pointer = targetIndex;

            for (int i = 0; i < sliceLength; i++)
            {
                pointer = (targetIndex + i) % this.Children.Count;

                UIElement element = this.Children[pointer];

                double offsetX = i * ItemWidth;

                Visual elementVisual = ElementCompositionPreview.GetElementVisual(element);

                //Do not animate elements outside of the viewport.
                if (Math.Abs(offsetX) > _viewportRect.Rect.Width)
                {
                    elementVisual.Offset = new Vector3((float)offsetX, elementVisual.Offset.Y, elementVisual.Offset.Z);
                }
                else
                {
                    var scalarAnimation = _carouselCompositor.CreateScalarKeyFrameAnimation();
                    scalarAnimation.Duration = TimeSpan.FromMilliseconds(Duration);
                    scalarAnimation.InsertKeyFrame(1f, (float)offsetX);
                    elementVisual.StartAnimation("Offset.X", scalarAnimation);
                }
            }

            for (int i = 1; i < sliceLength; i++)
            {
                pointer = (pointer + 1) % this.Children.Count;

                UIElement element = this.Children[pointer];

                double offsetX = (i - sliceLength) * ItemWidth;

                Visual elementVisual = ElementCompositionPreview.GetElementVisual(element);

                //Do not animate elements outside of the viewport.
                if (Math.Abs(offsetX) > _viewportRect.Rect.Width)
                {
                    elementVisual.Offset = new Vector3((float)offsetX, elementVisual.Offset.Y, elementVisual.Offset.Z);
                }
                else
                {
                    var scalarAnimation = _carouselCompositor.CreateScalarKeyFrameAnimation();
                    scalarAnimation.Duration = TimeSpan.FromMilliseconds(Duration);
                    scalarAnimation.InsertKeyFrame(1f, (float)offsetX);
                    elementVisual.StartAnimation("Offset.X", scalarAnimation);
                }
            }
        }

        /// <summary>
        /// Shift items with classic TranslateTransforms.
        /// </summary>
        /// <param name="targetIndex"></param>
        private void ShiftItems(int targetIndex)
        {
            if (targetIndex < 0 || this.Children.Count <= 0)
                return;

            int sliceLength = (int)Math.Ceiling((double)this.Children.Count / 2);

            int pointer = targetIndex;

            for (int i = 0; i < sliceLength; i++)
            {
                pointer = (targetIndex + i) % this.Children.Count;

                UIElement element = this.Children[pointer];

                double offsetX = i * ItemWidth;

                element.RenderTransform = new TranslateTransform { X = offsetX };
            }

            for (int i = 1; i < sliceLength; i++)
            {
                pointer = (pointer + 1) % this.Children.Count;

                UIElement element = this.Children[pointer];

                double offsetX = (i - sliceLength) * ItemWidth;

                element.RenderTransform = new TranslateTransform { X = offsetX };
            }
        }

        #region Public methods
        
        /// <summary>
        /// Move forward.
        /// </summary>
        public void MoveForward()
        {
            if (SelectedIndex == this.Children.Count - 1)
            {
                SelectedIndex = 0;
            }
            else
            {
                this.SelectedIndex += 1;
            }
        }

        /// <summary>
        /// Move backward.
        /// </summary>
        public void MoveBackward()
        {
            if (SelectedIndex == 0)
            {
                SelectedIndex = this.Children.Count - 1;
            }
            else
            {
                this.SelectedIndex -= 1;
            }
        }

        #endregion


        protected override Size MeasureOverride(Size availableSize)
        {
            _viewportRect = this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, availableSize.Width, availableSize.Height) };

            foreach (var uiElement in this.Children)
            {
                var container = (FrameworkElement)uiElement;

                container.Measure(new Size(ItemWidth, ItemHeight));
            }
            return (availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _viewportRect = this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };

            if (SelectedIndex == -1 || !this.Children.Any() || SelectedIndex >= this.Children.Count)
                return finalSize;
            
            var selectedElement = this.Children[this.SelectedIndex];

            Double centerX = (finalSize.Width / 2) - (ItemWidth / 2);
            Double centerY = (finalSize.Height - selectedElement.DesiredSize.Height) / 2;

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement element = this.Children[i];

                if (double.IsNaN(element.DesiredSize.Width) || double.IsNaN(element.DesiredSize.Height))
                    continue;

                var rect = new Rect(centerX, centerY, element.DesiredSize.Width, element.DesiredSize.Height);

                element.Arrange(rect);

            }

            if (DesignMode.DesignModeEnabled)
                return finalSize;

            ShiftItemsAnimatedly(SelectedIndex);

            return finalSize;
        }
    }
}