// JavaScript Code

// Code adapted from a YouTube video Developer Filip, 2021
// Method to scroll to the top of the page
// Implemented using a button that appears when scrolled down the page
document.getElementById("to_top_button").style.display = 'none'
window.onscroll = () => {
    if (window.scrollY >= 400) {
        document.getElementById("to_top_button").style.display = 'unset';
    }

    if (window.scrollY <= 200) {
        document.getElementById("to_top_button").style.display = 'none';
    }
}
const toTop = () => window.scrollTo({ top: 0, behavior: 'smooth' })
