`default_nettype none

// define a Count module
module top(input wire IN,
			  input wire CLK,
		     output wire [21:0] OUT,
			  output wire LED
		);	
	
	
	assign LED= ~IN;
	
	OPNDRN i0 (.in(IN), .out(OUT[0]));
	OPNDRN i1 (.in(IN), .out(OUT[1]));
	OPNDRN i2 (.in(IN), .out(OUT[2]));
	OPNDRN i3 (.in(IN), .out(OUT[3]));
	OPNDRN i4 (.in(IN), .out(OUT[4]));
	OPNDRN i5 (.in(IN), .out(OUT[5]));
	OPNDRN i6 (.in(IN), .out(OUT[6]));
	OPNDRN i7 (.in(IN), .out(OUT[7]));
	OPNDRN i8 (.in(IN), .out(OUT[8]));
	OPNDRN i9 (.in(IN), .out(OUT[9]));
	OPNDRN i10 (.in(IN), .out(OUT[10]));
	OPNDRN i11 (.in(IN), .out(OUT[11]));
	OPNDRN i12 (.in(IN), .out(OUT[12]));
	OPNDRN i13 (.in(IN), .out(OUT[13]));
	OPNDRN i14 (.in(IN), .out(OUT[14]));
	OPNDRN i15 (.in(IN), .out(OUT[15]));
	OPNDRN i16 (.in(IN), .out(OUT[16]));
	OPNDRN i17 (.in(IN), .out(OUT[17]));
	OPNDRN i18 (.in(IN), .out(OUT[18]));
	OPNDRN i19 (.in(IN), .out(OUT[19]));
	OPNDRN i20 (.in(IN), .out(OUT[20]));
	OPNDRN i21 (.in(IN), .out(OUT[21]));


endmodule
