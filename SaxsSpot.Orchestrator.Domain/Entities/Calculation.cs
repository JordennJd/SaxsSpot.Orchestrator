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
    public float QVectorFrom { get; set; }

    [Column("q_vector_to")]
    public float QVectorTo { get; set; }

    [Column("q_space_method")]
    public int QSpaceMethod { get; set; }

    [Column("q_space_parameter")]
    public float QSpaceParameter { get; set; }

    [Column("phi_vector_from")]
    public float? PhiVectorFrom { get; set; }

    [Column("phi_vector_to")]
    public float? PhiVectorTo { get; set; }

    [Column("phi_space_method")]
    public int? PhiSpaceMethod { get; set; }

    [Column("phi_space_parameter")]
    public float? PhiSpaceParameter { get; set; }

    [Column("theta_vector_from")]
    public float? ThetaVectorFrom { get; set; }

    [Column("theta_vector_to")]
    public float? ThetaVectorTo { get; set; }

    [Column("theta_space_method")]
    public int? ThetaSpaceMethod { get; set; }

    [Column("theta_space_parameter")]
    public float? ThetaSpaceParameter { get; set; }

    [Column("input_date")]
    public DateTime InputDate { get; set; }

    [Column("calculate_start")]
    public DateTime CalculateStart { get; set; }

    [Column("calculate_end")]
    public DateTime CalculateEnd { get; set; }
}