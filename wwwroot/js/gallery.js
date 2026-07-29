window.MamiaGallery = (function () {
  function initGallery() {
    const galleryItems = document.querySelectorAll('.gallery-item');
    galleryItems.forEach(function (item, index) {
      item.style.setProperty('transition-delay', (index % 4) * 25 + 'ms');
    });
  }

  return { initGallery: initGallery };
})();
