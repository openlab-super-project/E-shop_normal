import { Component, Inject, OnInit, signal} from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgClass, NgFor, NgIf, NgSwitch, NgSwitchCase } from '@angular/common';
import { CartService } from '../shopping-cart/cart.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { StarRatingComponent } from '../star-rating/star-rating.component';
import { validateHorizontalPosition } from '@angular/cdk/overlay';

@Component({
    selector: 'app-products-detail',
    templateUrl: './products-detail.component.html',
    styleUrls: ['./products-detail.component.css'],
    standalone: true,
    imports: [NgIf, NgFor, NgClass, NgSwitch, NgSwitchCase, ReactiveFormsModule, RouterLink, StarRatingComponent],
    providers: [MatSnackBar, StarRatingComponent]
})
export class ProductsDetailComponent implements OnInit {
    public productInfo: ProductsDTO = { //namiesto array vytvorim objekt, ktory ma ProductsDTO interface 
        productId: 0,
        productName: '',
        productDescription: '',
        price: 0,
        productCategory: '',
        productImage0: '',
        productImage1: '',
        productImage2: '',
        quantity: 0
    }; 
    public productName: string = '';
    public currentImagePosition: number = 0;
    public currentPageURL: string = '';
    public reviewsData: ReviewsDTO[] = [];
    productRating: number = 0;

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private route: ActivatedRoute, private CartService: CartService, private snackBar: MatSnackBar, private Router: Router, private StarRating: StarRatingComponent) {}

    reviewForm = new FormGroup({
        reviewComment: new FormControl('', Validators.required),
    });
    
    addToCart(){
        this.CartService.addToCart(this.productInfo);
        this.snackBar.open("Your product has been added to the cart!", "", { duration: 1500, }); 
    }

    positionLeft(){
        if(this.currentImagePosition > 0){
            this.currentImagePosition -= 1;
        }
    }
    positionRight(){
        if(this.currentImagePosition < 2){
            this.currentImagePosition += 1;
        }
    }

    onSubmit(){
        if(this.reviewForm.valid && this.productRating > 0){
            const routeParams = this.route.snapshot.paramMap;
            this.productName = String(routeParams.get('productName'));

            let reviewComment = this.reviewForm.value.reviewComment;
            let starRating = this.productRating;

            this.createReview(reviewComment, "Anonymous", this.productName, starRating).subscribe(newReview =>{
                const reviewDto: ReviewsDTO = newReview as ReviewsDTO; //povedat newReview ze je typu ReviewsDTO
                this.reviewsData.push(reviewDto);
            }
            );
            this.reviewForm.reset();
            this.productRating = 0;
        }
    }

    onRatingChange(rating: number) {
        this.productRating = rating;
    }

    createReview(reviewCommentBE: string, reviewCreatorBE: string, reviewdProductBE: string, starRatingBE: number) {
        const url = `${this.baseUrl}reviews/create-review`;
        const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        return this.http.put(url, { ReviewComment: reviewCommentBE, ReviewCreator: reviewCreatorBE, ReviewedProduct: reviewdProductBE, StarRating: starRatingBE }, { headers });
    }

    getReviews(productName: string): Observable<ReviewsDTO[]> { 
        let queryParams = new HttpParams();
        queryParams = queryParams.append("productName", productName);
        return this.http.get<ReviewsDTO[]>(this.baseUrl + 'reviews/getReviews', { params: queryParams });
    }
    
    getProductInfo(productName: string): Observable<ProductsDTO> { 
        let queryParams = new HttpParams();
        queryParams = queryParams.append("productName", productName);
        return this.http.get<ProductsDTO>(this.baseUrl + 'products/getProductInfo', { params: queryParams });
    }

    ngOnInit(): void {
        const routeParams = this.route.snapshot.paramMap;
        this.productName = String(routeParams.get('productName'));

        this.getProductInfo(this.productName).subscribe(
            result => {
                this.productInfo = result;
            },
            error => console.error(error)
        );
        this.getReviews(this.productName).subscribe(
           result => {
               this.reviewsData = result;
           },
           error => console.error(error)
        );
    }
}
export interface ProductsDTO {
    productId: number;
    productName: string;
    productDescription: string;
    price: number;
    productCategory: string;
    productImage0: string;
    productImage1: string;
    productImage2: string;
    quantity: number;
}
export interface ReviewsDTO{
    reviewId: number;
    reviewComment: string;
    reviewCreator: string;
    reviewedProduct: string;
    starRating: number;
}
