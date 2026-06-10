# EDSF UX/UI + Dashboard Design

## Objectives
- Modern, professional interface for ERP+CRM desktop app
- Dashboard with KPIs and charts replacing static home page
- Consistent design system across all pages

## Design System

### Color Palette
- Primary: `#1565C0` (blue 800)
- Primary Dark: `#0D47A1`
- Secondary: `#37474F` (blue-grey 800)
- Success: `#2E7D32`
- Warning: `#F9A825`
- Danger: `#C62828`
- Info: `#00838F`
- Background: `#F5F7FA`
- Surface: `#FFFFFF`
- Text Primary: `#212529`
- Text Secondary: `#6C757D`
- Border: `#DEE2E6`

### Navbar
- Fixed-top, dark background (`#1a237e`)
- Dropdown items with icons (Bootstrap Icons)
- Hover + click dropdown
- Search bar centered

### Cards
- All content sections wrapped in `.card` with `.card-body`
- Shadow: `0 2px 8px rgba(0,0,0,0.08)`
- Border-radius: `12px`

### Tables
- Inside cards
- Bootstrap `table-hover`
- Status badges (`.badge` with contextual colors)
- Action buttons with icons

### Forms
- Grid layout (2 columns on desktop)
- Consistent spacing
- Inline validation feedback

## Dashboard (Home.razor)

### KPI Row
4 cards showing current month metrics:
- Revenue (total invoiced this month)
- Pending Invoices (count)
- New Leads (this month)
- Low Stock Items (count)

### Charts (Chart.js CDN)
- Line chart: Revenue trend (last 6 months)
- Bar chart: Top customers by revenue
- Funnel/pipeline: Opportunities by stage
- Doughnut: Expense distribution

### Recent Activity
- Latest 5 invoices
- Latest 5 payments
- Latest 5 leads

## Navigation
- Bootstrap Icons added to all menu items
- Dropdown hover+click (Bootstrap JS)
- Active page highlighting
- Search input in navbar

## Implementation Order
1. Download Bootstrap Icons + Chart.js
2. Update `index.html` with new CSS/JS deps
3. Rewrite `app.css` with design system
4. Rewrite `Home.razor` as Dashboard
5. Update `NavMenu.razor` with icons
6. Update list/table pages with card wrapping + badges
