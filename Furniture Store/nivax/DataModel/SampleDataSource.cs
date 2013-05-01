using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Modern Furniture",
                 "Modern Furniture",
                 "Assets/10.jpg",
                 "Furniture is the mass noun for the movable objects intended to support various human activities such as seating and sleeping. Furniture is also used to hold objects at a convenient height for work (as horizontal surfaces above the ground), or to store things.");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Bean Bag",
                 "Bean Bag",
                 "Assets/11.jpg",
                 "Designed by an Italian company named Zanotta in 1969 beanbags have become a globally recognised piece of furniture. The first beanbag chairs were called Soccos which were pear-shaped leather bags filled with styrofoam beans and are still in production today.",
                 "\n\n\n\n\n\n\n\n\nDesigned by an Italian company named Zanotta in 1969 beanbags have become a globally recognised piece of furniture. The first beanbag chairs were called Soccos which were pear-shaped leather bags filled with styrofoam beans and are still in production today.\n\nPiero Gatti, Cesare Paolini and Franco Teodoro are credited with being the original designers at Zanotta, it's said they noticed that staff would sit on bags filled with styrofoam during their coffee and cigarette breaks.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Chaise Longue",
                 "Chaise Longue",
                 "Assets/12.jpg",
                 "In modern French the term chaise longue can refer to any long reclining chair such as a deckchair. A literal translation in English is long chair. In the United States the term lounge chair is also used to refer to any long reclining chair.",
                 "\n\n\n\n\n\n\n\n\nAncient Greek art depicts gods and goddesses lounging in this type of chair. The modern Greek word symposion comes from sympinein, which means to drink together. In ancient Greece this word conveyed the idea of a party with music and conversation. The principal item of furniture for a symposium is the kline, a form of daybed. The Greeks changed from the normal practice of sitting at a table to the far more distinctive practice of reclining on couches as early as the 8th century BC.\n\nThe Romans also used a daybed for reclining in the daytime and to sleep on at night. Developed from the Greek prototype, the Roman daybed was designed with legs carved in wood or cast bronze. The Romans also adapted a chaise longue style chair for the accubatio (the act of reclining during a meal). At Roman banquets, the usual number of persons occupying each bed was three, with three daybeds forming three sides of a small square, so that the triclinium (the dining-room of a Roman residence) allowed for a party of nine. The Romans did not practice upholstery, so the couches were made comfortable with pillows, loose covers and animal skins.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Ottoman",
                 "Ottoman",
                 "Assets/13.jpg",
                 "An ottoman is a piece of furniture consisting of a padded, upholstered seat or bench, usually having neither a back nor arms, often used as a stool or footstool, or in some cases as a coffee table. Ottomans are often sold as coordinating furniture with armchairs or gliders.",
                 "\n\n\n\n\n\n\n\n\nThe ottoman was brought to Europe from Turkey in the late 18th century. The word ottomane to refer to furniture appeared by at least 1729 in French. The first known recorded use in English occurs in one of Thomas Jefferson's memorandum books from 1789 Paid. for an Ottomane of velours d'Utrecht. \n\nIn Turkey, an ottoman was the central piece of family seating, and was piled with cushions. In Europe, the ottoman was first designed as a piece of fitted furniture that wrapped around three walls of a room. The ottoman evolved into a smaller version that fit into the corner of a room.\n\nOttomans took on a circular or octagonal shape through the 19th century, with seating divided in the center by arms or a central, padded column that might hold a plant or statue. As night clubs became more popular, so did the ottoman which began to have hinged seats underneath to hold magazines.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Stool",
                 "Stool",
                 "Assets/14.jpg",
                 "A stool is one of the earliest forms of seat furniture. It bears many similarities to a chair. It consists of a single seat, without back or armrests, on a base of either three or four legs.",
                 "\n\n\n\n\n\n\n\n\nA stool is one of the earliest forms of seat furniture. It bears many similarities to a chair. It consists of a single seat, without back or armrests, on a base of either three or four legs. A stool is distinguished from chairs by their lack of arms and a back. Variants exist with any number of legs from one to five. Some people call these various stools backless chairs.The origins of stools are lost in time although they are known to be one of the earliest forms of wooden furniture. Percy Macquoid claims that the turned stool was introduced from Byzantium by the Varangian guard, and thus through Norse culture into Europe, reaching England via the Normans.\n\nTable and stools from Bulgaria.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Recliner",
                 "Recliner",
                 "Assets/15.jpg",
                 "A recliner is an armchair or sofa that reclines when the occupant lowers the chair's back and raises its front. It has a backrest that can be tilted back, and often a footrest that may be extended by means of a lever on the side of the chair, or may extend automatically when the back is reclined.",
                 "\n\n\n\n\n\n\n\n\nA recliner is an armchair or sofa that reclines when the occupant lowers the chair's back and raises its front. It has a backrest that can be tilted back, and often a footrest that may be extended by means of a lever on the side of the chair, or may extend automatically when the back is reclined.\n\nA recliner is also known as a reclining chair, lounger and an armchair.\n\nModern recliners often feature an adjustable headrest, lumbar support and an independent footstool that adjusts with the weight and angle of the user's legs to maximize comfort. Recliners that are wheelchair accessible are also available.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Fauteuil",
                 "Fauteuil",
                 "Assets/16.jpg",
                 "A fauteuil is made of wood, and frequently with carved relief ornament. It is typically upholstered on the seat, the seat back and on the arms (manchettes). Some fauteuils have a valenced front seat rail which is padding that extends slightly over the apron. The exposed wooden elements are often gilded or otherwise painted.",
                 "\n\n\n\n\n\n\n\n\n\n\n\n\nA fauteuil is made of wood, and frequently with carved relief ornament. It is typically upholstered on the seat, the seat back and on the arms (manchettes). Some fauteuils have a valenced front seat rail which is padding that extends slightly over the apron. The exposed wooden elements are often gilded or otherwise painted.\n\nA fauteuil is a style of open-arm chair with a primarily exposed wooden frame originating in France in the early 18th century.",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Perfect Furniture",
                 "Perfect Furniture",
                 "Assets/20.jpg",
                 "Furniture has been a part of the human experience since the development of non-nomadic cultures. Evidence of furniture survives from the Neolithic Period and later in antiquity in the form of paintings, such as the wall Murals discovered at Pompeii; sculpture, and examples have been excavated in Egypt and found in tombs in Ghiordes, in modern day Turkey.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Conventional Household",
                 "Conventional Household",
                 "Assets/21.jpg",
                 "Traditional furniture includes different styles of furniture with a variety of styles overlapping each other.",
                 "\n\n\n\n\n\n\n\n\nTraditional furniture includes different styles of furniture with a variety of styles overlapping each other. Generally, traditional furniture is made from darker woods plus more ornate in design. Scroll work and also detailed carving generally go with this type of furniture and traditional furniture is a favorite of many creative designers due to the artistic design. Types of furniture that might be considered traditional are Chippendale and Queen Anne varieties of furniture.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Transitional Furniture",
                 "Transitional Furniture",
                 "Assets/22.jpg",
				 "Transitional furniture is exactly what its name suggests. Transitional furniture is a style or type of furniture which mixes both modern and traditional elements collectively.",
                 "\n\n\n\n\n\n\n\n\nTransitional furniture is exactly what its name suggests. Transitional furniture is a style or type of furniture which mixes both modern and traditional elements collectively to give you a pleasant mixture of performance, form and style of both styles. While holding somewhat to standard materials however leaning towards contemporary styling, transitional furniture can be a great balance for a smaller home that might turn out to be overwhelmed with the detail along with ornateness of traditional furniture.",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "Daybed",
                 "Daybed",
                 "Assets/23.jpg",
                 "Daybeds are used as beds as well as for lounging, reclining and seating in common rooms. Their frames can be made out of wood, metal or a combination of wood and metal.",
                 "\n\n\n\n\n\n\n\n\nDaybeds are used as beds as well as for lounging, reclining and seating in common rooms. Their frames can be made out of wood, metal or a combination of wood and metal.They are a cross between chaise longue, couch and a bed.\n\nDaybeds typically feature a back and sides and come in Twin Size (39 in × 75 in = 99 cm × 191 cm). Often daybeds will also feature a trundle to expand sleeping capacity.\n\nMany of today's daybeds employ a linkspring as the support system for the mattress. The linkspring is a rectangular metal frame (roughly the footprint of the mattress) with cross supports. A wire or polyester / nylon mesh held in place by a network of springs lays across the top of the linkspring. The linkspring design provides support and creates clearance underneath a daybed for storage.\n\nA recent development in infant beds is a bed that converts into a daybed by the removal of one side, ensuring the furniture is useful for more than a couple of years.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Futon",
                 "Futon",
                 "Assets/24.jpg",
                 "A futon is traditional Japanese bedding consisting of padded mattresses and quilts pliable enough to be folded and stored away during the day, allowing the room to serve for purposes other than as a bedroom.",
                 "\n\n\n\n\n\n\n\n\nA futon is a flat mattress with a fabric exterior stuffed with cotton, wool, or synthetic batting that makes up a Japanese bed. Futons are sold in Japan at speciality stores called futon'ya as well as at department stores. They are often sold in sets that include the futon mattress (shikibuton), a comforter (kakebuton) or blanket (mōfu), a summer blanket resembling a large towel ( taoruketto), and a pillow ( makura) generally filled with beans, buckwheat chaff, or plastic beads.[citation needed] Futons are designed to be placed on tatami flooring, and are traditionally folded away and stored in a closet during the day to allow the tatami to breathe and to allow for flexibility in the use of the room. \n\nFutons must be aired in sunlight regularly, especially if not put away during the day. In addition, many Japanese beat their futons regularly to prevent the padding from matting. They use a futon tataki , a special instrument, traditionally made from bamboo, resembling a Western carpet beater",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Hammock",
                 "Hammock",
                 "Assets/25.jpg",
                 "A hammock is a sling made of fabric, rope, or netting, suspended between two points, used for swinging, sleeping, or resting. It normally consists of one or more cloth panels.",
                 "\n\n\n\n\n\n\n\n\nA hammock is a sling made of fabric, rope, or netting, suspended between two points, used for swinging, sleeping, or resting. It normally consists of one or more cloth panels, or a woven network of twine or thin rope stretched with ropes between two firm anchor points such as trees or posts. Hammocks were developed by native inhabitants of Central and South America for sleeping.\n\n Later, they were used aboard ships by sailors to enable comfort and maximize available space, and by explorers or soldiers traveling in wooded regions. Today they are popular around the world for relaxation; they are also used as a lightweight bed on camping trips. The hammock is often seen as symbol of summer, leisure, relaxation and simple, easy living.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Headboard",
                 "Headboard",
                 "Assets/26.jpg",
                 "The headboard is a piece of furniture that attaches to the head of a bed. Historically, they served to isolate sleepers from drafts and cold in less insulated buildings, and thus were made of wood, which is less conductive than stone or brick.",
                 "\n\n\n\n\n\n\n\n\nThe headboard is a piece of furniture that attaches to the head of a bed. Historically, they served to isolate sleepers from drafts and cold in less insulated buildings, and thus were made of wood, which is less conductive than stone or brick. Constructed to create space from the wall (via thicker end pillars) they allowed falling colder air to sink to the floor rather than onto the bed.\n\nToday in better heated and insulated residences headboards serve chiefly aesthetic and utilitarian functions. They may include storage space for books and personal items, and conveniences such as lights and telephone. Those of hospital beds may incorporate critical care functions. A headboard may often be complemented by a footboard for aesthetic balance.",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
