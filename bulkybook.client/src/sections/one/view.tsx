import Box from '@mui/material/Box';
import { alpha } from '@mui/material/styles';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';

import { useSettingsContext } from 'src/components/settings';

import { DataGrid, GridColDef, GridActionsCellItem } from '@mui/x-data-grid';
import { Breadcrumbs, Button, Card, Link } from '@mui/material';

import Iconify from 'src/components/iconify';

import axiosInstance, { fetcher, endpoints } from 'src/utils/axios';
import { useNavigate } from 'react-router-dom';


import { useEffect, useState } from 'react';

// ----------------------------------------------------------------------

// Define the structure for the grid rows based on your Product type
type Product = {
  id: number;
  title: string;
  author: string;
  listPrice: number;
  price: number;
};

// Define columns for the DataGrid
const columns: GridColDef<Product>[] = [
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
      <GridActionsCellItem icon={<Iconify icon="solar:eye-bold" />} label="View" />,
      <GridActionsCellItem icon={<Iconify icon="solar:pen-bold" />} label="Edit" />,
      <GridActionsCellItem icon={<Iconify icon="solar:trash-bin-trash-bold" />} label="Delete" sx={{ color: 'error.main' }} />,
    ],
  },
];

export default function OneView() {
  const settings = useSettingsContext();
  const [products, setProducts] = useState<Product[]>([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();


  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const productList = await fetcher(endpoints.product.list);
        console.log('Fetched Products:', productList); // Debugging line
        setProducts(productList);
      } catch (err) {
        console.error('Failed to fetch products:', err);
        setError(err.message || 'Failed to fetch products');
      }
    };

    fetchProducts();
  }, []);

  // Map Products to RowData for the grid
  const rows: Product[] = products.map((product) => ({
    id: product.id,
    title: product.title,
    author: product.author,
    listPrice: product.listPrice,
    price: product.price
  }));

  console.log('Rows for DataGrid:', rows); // Debugging line

  const handleUpsertClick = (id: number|null) => {
    navigate(`/product/${id}?`);
  };


  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <Container maxWidth={settings.themeStretch ? false : 'xl'}>
      <Typography variant="h4"> Products </Typography>


      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mt: 2 }}>
        <Breadcrumbs>
          <Link underline="hover" color="inherit" href="/">
            Product
          </Link>
          <Link
            underline="hover"
            color="inherit"
            href="/material-ui/getting-started/installation/"
          >
            List
          </Link>
          {/* <Typography color="text.primary">Breadcrumbs</Typography> */}
        </Breadcrumbs>

        <Button variant="contained" color="primary" onClick={() => handleUpsertClick(null)}>
          Add Product
        </Button>
      </Box>

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
