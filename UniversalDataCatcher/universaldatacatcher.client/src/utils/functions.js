export const handleNavigation = (e, url) => {
  e.stopPropagation();
  window.open(url, "_blank");
};
