import Box from '@mui/material/Box';
import { alpha } from '@mui/material/styles';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';

import { useSettingsContext } from 'src/components/settings';

import { countries } from 'src/assets/data';

import { DataGrid, GridColDef, GridActionsCellItem } from '@mui/x-data-grid';
import { Card } from '@mui/material';

import Iconify from 'src/components/iconify';

import Products from 'src/api_data/Product';

// ----------------------------------------------------------------------



// Define the structure for the grid rows based on your Product type
type RowData = {
  id: number
  title: string;
  author: string;
  listPrice: number;
  price: number;
};

// Map Products to RowData for the grid
const rows: RowData[] = Products.map((product) => ({
  id: product.id,
  title: product.title,
  author: product.author,
  listPrice: product.listPrice,
  price: product.price
}));

// Define columns for the DataGrid
const columns: GridColDef<RowData>[] = [
  { field: 'title', headerName: 'Title', width: 200 },
  { field: 'author', headerName: 'Author', width: 160 },
  { field: 'listPrice', headerName: 'List Price', type: 'number', width: 120 },
  { field: 'price', headerName: 'Price', type: 'number', width: 110 },
  {
    type: 'actions',
    field: 'actions',
    headerName: 'Actions',
    width: 200,
    getActions: () => [
      <GridActionsCellItem
        icon={<Iconify icon="solar:eye-bold" />}
        label="View"
      />,
      <GridActionsCellItem
        icon={<Iconify icon="solar:pen-bold" />}
        label="Edit"
      />,
      <GridActionsCellItem
        icon={<Iconify icon="solar:trash-bin-trash-bold" />}
        label="Delete"
        sx={{ color: 'error.main' }}
      />,
    ],
  },
];


export default function OneView() {
  const settings = useSettingsContext();

  return (
    <Container maxWidth={settings.themeStretch ? false : 'xl'}>
      <Typography variant="h4"> Products </Typography>

      <Box
        sx={{
          mt: 5,
          width: 1,
          height: 400,
          borderRadius: 2,
          bgcolor: (theme) => alpha(theme.palette.grey[500], 0.04),
          border: (theme) => `dashed 1px ${theme.palette.divider}`,
        }}
      >

        <Card
          sx={{
            height: { xs: 1, md: 2, lg: 1 },
            flexGrow: { md: 1 },
            display: { md: 'flex' },
            flexDirection: { md: 'column' },

          }}
        >
          <DataGrid
            rows={rows}
            columns={columns}
            pageSize={5}
            rowsPerPageOptions={[5, 10, 25]}
            checkboxSelection
          />
        </Card>
      </Box>
    </Container>
  );
}
