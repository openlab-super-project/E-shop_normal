import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { TestService } from '../test.service';
import { AsyncPipe, CommonModule, NgForOf, NgIf } from '@angular/common';
import { MatList, MatListItem } from '@angular/material/list';
import { MatCell, MatCellDef, MatColumnDef, MatHeaderCell, MatHeaderCellDef, MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef, MatTable, MatTableDataSource } from '@angular/material/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthenticationService, RoleDTO, UserDTO } from '../api-authorization/authentication.service';
import { Router } from '@angular/router';
import { FavoriteProductsService } from '../favorite-products/favorite-products.service';
import { FavoriteProductsComponent } from '../favorite-products/favorite-products.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    NgIf,
    AsyncPipe,
    MatList,
    MatListItem,
    NgForOf,
    MatTable,
    MatHeaderCell,
    MatCell,
    MatCellDef,
    MatHeaderCellDef,
    MatColumnDef,
    MatRowDef,
    MatHeaderRowDef,
    MatHeaderRow,
    MatRow,
    CommonModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  authService = inject(AuthenticationService);
  user: UserDTO;
  role: RoleDTO;
  roleName: string = '';

  isLoading: boolean = true;

  constructor(private router: Router, private favoriteProductsService: FavoriteProductsService){}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  ngOnInit(): void {
    if(this.authService.authenticated()){
      this.authService.getCurrentUser().subscribe(result =>{
          this.user = result;
          this.authService.getRole(this.user.id).subscribe(result => {
              this.role = result;
              this.isLoading = false;
              if(this.role != null){
                  this.roleName = this.role.name;
              }
          });
      })
   }
  }
}