namespace SaxsSpot.Orchestrator.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("calculation")]
public class Calculation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("nanosystem_id")]
    public Guid NanosystemId { get; set; }

    [Column("object_id")]
    public Guid ObjectId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("q_vector_from")]
    public double QVectorFrom { get; set; }

    [Column("q_vector_to")]
    public double QVectorTo { get; set; }

    [Column("q_space_method")]
    public int QSpaceMethod { get; set; }
    
    [Column("q_scale_method")]
    public int QScaleMethod { get; set; }

    [Column("q_space_parameter")]
    public double QSpaceParameter { get; set; }

    [Column("phi_vector_from")]
    public double? PhiVectorFrom { get; set; }

    [Column("phi_vector_to")]
    public double? PhiVectorTo { get; set; }

    [Column("phi_space_method")]
    public int? PhiSpaceMethod { get; set; }
    
    [Column("phi_scale_method")]
    public int? PhiScaleMethod { get; set; }

    [Column("phi_space_parameter")]
    public double? PhiSpaceParameter { get; set; }

    [Column("theta_vector_from")]
    public double? ThetaVectorFrom { get; set; }

    [Column("theta_vector_to")]
    public double? ThetaVectorTo { get; set; }

    [Column("theta_space_method")]
    public int? ThetaSpaceMethod { get; set; }

    [Column("theta_scale_method")]
    public int? ThetaScaleMethod { get; set; }
    
    [Column("theta_space_parameter")]
    public double? ThetaSpaceParameter { get; set; }

    [Column("input_date")]
    public DateTime InputDate { get; set; }

    [Column("calculate_start")]
    public DateTime CalculateStart { get; set; }

    [Column("calculate_end")]
    public DateTime CalculateEnd { get; set; }
}