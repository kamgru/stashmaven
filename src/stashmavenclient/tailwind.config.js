/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      gridTemplateColumns: {
        'catalog-items': 'minmax(20px, 50px) repeat(2, minmax(150px, 1fr)) repeat(2, minmax(30px, 0.5fr))',
        'select-inventory-item': 'minmax(20px, 50px) repeat(4, minmax(150px, 1fr)) repeat(4, minmax(30px, 0.5fr))'
      }
    },
  },
  plugins: [
    require('@tailwindcss/typography'),
    require('@tailwindcss/forms'),
  ],
}
