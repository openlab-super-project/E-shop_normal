import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, Inject, OnInit, inject } from '@angular/core';
import { AuthenticationService, UserDTO } from '../api-authorization/authentication.service';
import { NgFor, NgForOf } from '@angular/common';
import { ProductsDTO } from '../shopping-cart/cart.service';
import { Router, RouterLink } from '@angular/router';
import { FavoriteProductsService } from './favorite-products.service';
import { PaginationComponent } from '../pagination/pagination.component';
import { ProductsComponent } from '../products/products.component';

@Component({
  selector: 'app-favorite-products',
  standalone: true,
  imports: [NgFor, NgForOf, RouterLink, PaginationComponent],
  providers: [ProductsComponent],
  templateUrl: './favorite-products.component.html',
  styleUrl: './favorite-products.component.css'
})
export class FavoriteProductsComponent implements OnInit {
  favoriteProductsData: ProductsDTO[] = [];
  paginatedFavoriteProducts: ProductsDTO[] = [];

  authService = inject(AuthenticationService);
  user: UserDTO;

  currentPage: number = 1;
  totalItems: number = 0;
  limit: number = 3;

  isLoading: boolean = true;

  constructor(public FavProductsService: FavoriteProductsService, private router: Router){}
  
  onPageChange(page: number){
    this.currentPage = page;
    this.updateCurrentProducts();
  }
  updateCurrentProducts(){
    const startIndex = (this.currentPage - 1) * this.limit;
    const endIndex = startIndex + this.limit;
    this.paginatedFavoriteProducts = this.favoriteProductsData.slice(startIndex, endIndex);
  }
  test(){
    alert("Hi!")
  }
  
  ngOnInit(): void {
    if(this.authService.authenticated()){
      this.authService.getCurrentUser().subscribe(result =>{
          this.user = result;
          this.FavProductsService.getFavoriteProducts(this.user.id).subscribe(result => {
            this.favoriteProductsData = result;
            this.totalItems = this.favoriteProductsData.length;
            this.updateCurrentProducts();
            this.isLoading = false;
          });
      })
    }else{
      this.router.navigate(['/login']);
    }
  }
}
export interface FavoriteProductDTO{
  userId: string;
  productId: number;
}